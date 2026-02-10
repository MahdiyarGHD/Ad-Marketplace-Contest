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
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan BaseRetryInterval = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan MaxRetryInterval = TimeSpan.FromHours(6);
    private const double BackoffMultiplier = 1.5;

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
        var agentService = scope.ServiceProvider.GetRequiredService<IAgentService>();
        var analyticsQueue = scope.ServiceProvider.GetRequiredService<IAnalyticsUpdateQueue>();

        var channels = await dbContext.Channels
            .AsTracking()
            .Include(c => c.Agent)
            .Where(c => c.Status == ChannelStatusType.UnReady || c.Status == ChannelStatusType.PartiallyReady)
            .ToListAsync(ct);

        var now = DateTimeOffset.UtcNow;

        foreach (var channel in channels)
        {
            if (!ShouldRetry(channel.LastReadinessCheckAt, channel.CreatedAt, now))
                continue;

            try
            {
                await CheckAndFixChannelAsync(channel, dbContext, botClient, agentService, analyticsQueue, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed readiness check for channel {ChannelId}", channel.Id);
                channel.MarkReadinessChecked();
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }

    private async Task CheckAndFixChannelAsync(
        Database.Models.Channel channel,
        AdMarketDbContext dbContext,
        ITelegramBotClient botClient,
        IAgentService agentService,
        IAnalyticsUpdateQueue analyticsQueue,
        CancellationToken ct)
    {
        channel.MarkReadinessChecked();

        var botOk = await VerifyBotPermissionsAsync(botClient, channel.ChatId, ct);
        if (!botOk)
        {
            logger.LogWarning("Bot lacks required permissions in channel {ChannelId} (ChatId={ChatId})", channel.Id, channel.ChatId);
            channel.SetStatus(ChannelStatusType.UnReady);
            channel.DetachAgent();
            return;
        }

        if (channel.Status == ChannelStatusType.UnReady)
            channel.SetStatus(ChannelStatusType.PartiallyReady);

        var agentOk = channel.AgentId.HasValue
            && await VerifyAgentInChannelAsync(botClient, channel.ChatId, channel.Agent!, ct);

        if (agentOk)
        {
            channel.SetStatus(ChannelStatusType.Ready);
            logger.LogInformation("Channel {ChannelId} is now Ready", channel.Id);
            await analyticsQueue.EnqueueAsync(
                new Domain.Contracts.Common.AnalyticsUpdateMessage(channel.Id, DateTimeOffset.UtcNow), ct);
            return;
        }

        if (channel.AgentId.HasValue)
        {
            channel.DetachAgent();
            await dbContext.SaveChangesAsync(ct);
        }

        logger.LogInformation("Attempting to attach agent to channel {ChannelId}", channel.Id);
        var attachResult = await agentService.AttachAgentToChannel(channel.Id);

        if (!attachResult.IsError)
        {
            logger.LogInformation("Agent attached to channel {ChannelId}, now Ready", channel.Id);
            await analyticsQueue.EnqueueAsync(
                new Domain.Contracts.Common.AnalyticsUpdateMessage(channel.Id, DateTimeOffset.UtcNow), ct);
            return;
        }

        logger.LogWarning("First agent attach attempt failed for channel {ChannelId}, trying another agent", channel.Id);

        var retryResult = await agentService.AttachAgentToChannel(channel.Id);
        if (!retryResult.IsError)
        {
            logger.LogInformation("Second agent attached to channel {ChannelId}, now Ready", channel.Id);
            await analyticsQueue.EnqueueAsync(
                new Domain.Contracts.Common.AnalyticsUpdateMessage(channel.Id, DateTimeOffset.UtcNow), ct);
        }
        else
        {
            logger.LogWarning("All agent attach attempts failed for channel {ChannelId}", channel.Id);
        }
    }

    private static bool ShouldRetry(DateTimeOffset? lastCheck, DateTimeOffset createdAt, DateTimeOffset now)
    {
        if (!lastCheck.HasValue)
            return true;

        var timeSinceLastCheck = now - lastCheck.Value;
        var timeSinceCreation = now - createdAt;

        var interval = CalculateRetryInterval(timeSinceCreation);
        return timeSinceLastCheck >= interval;
    }

    private static TimeSpan CalculateRetryInterval(TimeSpan timeSinceCreation)
    {
        if (timeSinceCreation <= BaseRetryInterval)
            return BaseRetryInterval;

        var minutesSinceCreation = timeSinceCreation.TotalMinutes;
        var intervalMinutes = BaseRetryInterval.TotalMinutes;

        while (intervalMinutes * BackoffMultiplier < minutesSinceCreation)
        {
            intervalMinutes *= BackoffMultiplier;
            if (intervalMinutes >= MaxRetryInterval.TotalMinutes)
                return MaxRetryInterval;
        }

        return TimeSpan.FromMinutes(Math.Min(intervalMinutes, MaxRetryInterval.TotalMinutes));
    }

    private static async Task<bool> VerifyBotPermissionsAsync(ITelegramBotClient botClient, long chatId, CancellationToken ct)
    {
        try
        {
            var me = await botClient.GetMe(ct);
            var botMember = await botClient.GetChatMember(chatId, me.Id, ct);

            return botMember is ChatMemberAdministrator
            {
                CanPromoteMembers: true,
                CanInviteUsers: true,
                CanDeleteMessages: true,
                CanPostMessages: true,
                CanEditMessages: true,
            };
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException)
        {
            return false;
        }
    }

    private static async Task<bool> VerifyAgentInChannelAsync(
        ITelegramBotClient botClient,
        long chatId,
        Database.Models.Agent agent,
        CancellationToken ct)
    {
        if (!agent.IsActive)
            return false;

        try
        {
            var member = await botClient.GetChatMember(chatId, agent.UserId, ct);

            return member is ChatMemberAdministrator
            {
                CanPostMessages: true,
                CanEditMessages: true,
                CanDeleteMessages: true,
                CanInviteUsers: true,
                CanPinMessages: true,
            };
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException)
        {
            return false;
        }
    }
}
