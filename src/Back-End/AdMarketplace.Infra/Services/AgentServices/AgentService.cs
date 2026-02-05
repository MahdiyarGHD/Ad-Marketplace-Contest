using AdMarketplace.Database;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using TL;
using ChannelModel = AdMarketplace.Database.Models.Channel;
using ErrorOr;
using Error = ErrorOr.Error;
using Microsoft.Extensions.Logging;

namespace AdMarketplace.Infra.Services.AgentServices;

public class AgentService(
    AdMarketDbContext dbContext,
    ILogger<AgentService> logger,
    ITelegramBotClient botClient,
    IClientFactory clientFactory) : IAgentService
{
    public async Task<ErrorOr<ChannelModel>> AttachAgentToChannel(Guid channelId)
    {
        var channel = await dbContext.Channels
            .AsTracking()
            .FirstOrDefaultAsync(c => c.Id == channelId);

        if (channel is null)
            return Error.NotFound("Channel.NotFound", "Channel not found");

        var agents = await dbContext.Agents
            .Where(a => a.IsActive)
            .ToListAsync();

        if (agents.Count == 0)
            return Error.NotFound("Agent.NotFound", "No active agents available");

        var random = new Random();
        var agent = agents[random.Next(agents.Count)];

        channel.AttachAgent(agent.Id);
        await dbContext.SaveChangesAsync();

        WTelegram.Client? client = null;
        try
        {
            var clientResult = await clientFactory.CreateClientAsync(agent.Id);
            if (clientResult.IsError)
            {
                channel.SetStatus(ChannelStatusType.UnReady);
                channel.DetachAgent();
                await dbContext.SaveChangesAsync();
                return clientResult.Errors;
            }

            client = clientResult.Value;

            var loginResult = await client.LoginUserIfNeeded();
            if (loginResult is null)
            {
                channel.SetStatus(ChannelStatusType.UnReady);
                channel.DetachAgent();
                await dbContext.SaveChangesAsync();
                return Error.Failure("Agent.LoginFailed", "Failed to login agent");
            }

            var chatInfo = await botClient.GetChat(channel.ChatId);
            var inviteLink = chatInfo.InviteLink ?? (await botClient.CreateChatInviteLink(channel.ChatId)).InviteLink;

            await client.Messages_ImportChatInvite(inviteLink.Replace("https://t.me/+", ""));

            await botClient.PromoteChatMember(
                chatId: channel.ChatId,
                userId: agent.UserId,
                canPostMessages: true,
                canEditMessages: true,
                canDeleteMessages: true,
                canInviteUsers: true,
                canPinMessages: true);

            channel.SetStatus(ChannelStatusType.Ready);
            await dbContext.SaveChangesAsync();
            return channel;
        }
        catch (Exception ex)
        {
            channel.SetStatus(ChannelStatusType.UnReady);
            channel.DetachAgent();
            await dbContext.SaveChangesAsync();

            logger.LogError(ex, "Failed to attach agent {AgentId} to channel {ChannelId}", agent.Id, channel.Id);            
            return Error.Failure("Agent.AttachmentFailed", "Failed to attach agent to channel");
        }
        finally
        {
            client?.Dispose();
        }
    }
}
