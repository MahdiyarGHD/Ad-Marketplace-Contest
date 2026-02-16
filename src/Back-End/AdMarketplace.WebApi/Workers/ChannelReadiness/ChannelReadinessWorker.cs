using AdMarketplace.Database;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AdMarketplace.Workers.ChannelReadiness;

public class ChannelReadinessWorker(
    IServiceProvider serviceProvider,
    ILogger<ChannelReadinessWorker> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan ChannelThrottleDelay = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan BaseRetryInterval = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan MaxRetryInterval = TimeSpan.FromDays(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("ChannelReadinessWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessUnreadyChannelsAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error in channel readiness check cycle");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }

        logger.LogInformation("ChannelReadinessWorker stopped");
    }

    private async Task ProcessUnreadyChannelsAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AdMarketDbContext>();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        var analyticsQueue = scope.ServiceProvider.GetRequiredService<IAnalyticsUpdateQueue>();

        var channels = await dbContext.Channels
            .AsTracking()
            .Where(c => c.Status == ChannelStatusType.UnReady || c.Status == ChannelStatusType.PartiallyReady)
            .ToListAsync(ct);

        var now = DateTimeOffset.UtcNow;

        foreach (var channel in channels)
        {
            if (!ShouldRetry(channel, now))
                continue;

            try
            {
                await CheckAndFixChannelAsync(channel, dbContext, botClient, analyticsQueue, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed readiness check for channel {ChannelId}", channel.Id);
            }

            channel.MarkReadinessChecked();
            await dbContext.SaveChangesAsync(ct);
            await Task.Delay(ChannelThrottleDelay, ct);
        }
    }

    private async Task<bool> CheckAndFixChannelAsync(
        Database.Models.Channel channel,
        AdMarketDbContext dbContext,
        ITelegramBotClient botClient,
        IAnalyticsUpdateQueue analyticsQueue,
        CancellationToken ct)
    {
        var botOk = await VerifyBotPermissionsAsync(botClient, channel.ChatId, ct);
        if (!botOk)
        {
            logger.LogWarning("Bot not admin in channel {ChannelId} (ChatId={ChatId})", channel.Id, channel.ChatId);
            if (channel.Status != ChannelStatusType.UnReady)
                channel.SetStatus(ChannelStatusType.UnReady);
            if (channel.AgentId.HasValue)
                channel.DetachAgent();
            return false;
        }

        if (channel.Status == ChannelStatusType.UnReady)
            channel.SetStatus(ChannelStatusType.PartiallyReady);

        var activeAgents = await dbContext.Agents
            .Where(a => a.IsActive)
            .ToListAsync(ct);

        if (activeAgents.Count == 0)
        {
            logger.LogWarning("No active agents available for channel {ChannelId}", channel.Id);
            return false;
        }

        foreach (var agent in activeAgents)
        {
            var status = await GetAgentChannelStatusAsync(botClient, channel.ChatId, agent.UserId, ct);

            if (status == AgentChannelStatus.NotInChannel)
                continue;

            if (status == AgentChannelStatus.InChannelNeedsPromotion)
            {
                try
                {
                    await botClient.PromoteChatMember(
                        chatId: channel.ChatId,
                        userId: agent.UserId,
                        canPostMessages: true,
                        canEditMessages: true,
                        canDeleteMessages: true,
                        canInviteUsers: true,
                        canPinMessages: true);
                }
                catch (Telegram.Bot.Exceptions.ApiRequestException ex)
                {
                    logger.LogWarning(ex, "Failed to promote agent {AgentId} in channel {ChannelId}", agent.Id, channel.Id);
                    continue;
                }
            }

            channel.AttachAgent(agent.Id);
            channel.SetStatus(ChannelStatusType.Ready);
            logger.LogInformation("Channel {ChannelId} Ready — agent {AgentId}", channel.Id, agent.Id);
            await analyticsQueue.EnqueueAsync(
                new Domain.Contracts.Common.AnalyticsUpdateMessage(channel.Id, DateTimeOffset.UtcNow), ct);
            return true;
        }

        logger.LogInformation("No agent in channel {ChannelId}, attempting fresh attach", channel.Id);

        using var attachScope = serviceProvider.CreateScope();
        var agentService = attachScope.ServiceProvider.GetRequiredService<IAgentService>();
        var attachResult = await agentService.AttachAgentToChannel(channel.Id);

        if (attachResult.IsError)
        {
            logger.LogWarning("Agent attach failed for channel {ChannelId}: {Error}", channel.Id, attachResult.FirstError.Description);
            await dbContext.Entry(channel).ReloadAsync(ct);
            return false;
        }

        await dbContext.Entry(channel).ReloadAsync(ct);

        logger.LogInformation("Agent attached to channel {ChannelId}, now Ready", channel.Id);
        await analyticsQueue.EnqueueAsync(
            new Domain.Contracts.Common.AnalyticsUpdateMessage(channel.Id, DateTimeOffset.UtcNow), ct);
        return true;
    }

    private static bool ShouldRetry(Database.Models.Channel channel, DateTimeOffset now)
    {
        if (!channel.LastReadinessCheckAt.HasValue)
            return true;

        var timeSinceLastCheck = now - channel.LastReadinessCheckAt.Value;
        var stuckSince = channel.UpdatedAt ?? channel.CreatedAt;
        var stuckDuration = channel.LastReadinessCheckAt.Value - stuckSince;
        if (stuckDuration < TimeSpan.Zero)
            stuckDuration = TimeSpan.Zero;

        var interval = CalculateRetryInterval(stuckDuration);
        return timeSinceLastCheck >= interval;
    }

    private static TimeSpan CalculateRetryInterval(TimeSpan stuckDuration)
    {
        var intervalMinutes = BaseRetryInterval.TotalMinutes + stuckDuration.TotalMinutes * 0.1;
        return TimeSpan.FromMinutes(Math.Min(intervalMinutes, MaxRetryInterval.TotalMinutes));
    }

    private static async Task<bool> VerifyBotPermissionsAsync(ITelegramBotClient botClient, long chatId, CancellationToken ct)
    {
        try
        {
            var me = await botClient.GetMe(ct);
            var botMember = await botClient.GetChatMember(chatId, me.Id, ct);

            return botMember is ChatMemberAdministrator admin
                && admin.CanPromoteMembers
                && admin.CanInviteUsers
                && admin.CanDeleteMessages
                && admin.CanPostMessages == true
                && admin.CanEditMessages == true;
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException)
        {
            return false;
        }
    }

    private static async Task<AgentChannelStatus> GetAgentChannelStatusAsync(
        ITelegramBotClient botClient,
        long chatId,
        long agentUserId,
        CancellationToken ct)
    {
        try
        {
            var member = await botClient.GetChatMember(chatId, agentUserId, ct);

            if (member is ChatMemberAdministrator admin
                && admin.CanPostMessages == true
                && admin.CanEditMessages == true
                && admin.CanDeleteMessages
                && admin.CanInviteUsers
                && admin.CanPinMessages)
                return AgentChannelStatus.ReadyAdmin;

            if (member is ChatMemberAdministrator or ChatMemberMember)
                return AgentChannelStatus.InChannelNeedsPromotion;

            return AgentChannelStatus.NotInChannel;
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException)
        {
            return AgentChannelStatus.NotInChannel;
        }
    }

    private enum AgentChannelStatus
    {
        NotInChannel,
        InChannelNeedsPromotion,
        ReadyAdmin
    }
}
