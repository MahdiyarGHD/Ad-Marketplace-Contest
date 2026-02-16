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

            try
            {
                await client.Messages_ImportChatInvite(inviteLink.Replace("https://t.me/+", ""));
            }
            catch (RpcException rpcEx) when (rpcEx.Code == 400 && rpcEx.Message is "USER_ALREADY_PARTICIPANT" or "INVITE_HASH_EXPIRED")
            {
                logger.LogInformation("Agent {AgentId} already in channel {ChannelId}, skipping join", agent.Id, channel.Id);
            }
            catch (RpcException rpcEx) when (rpcEx.Code == 420 && rpcEx.Message.StartsWith("FLOOD_WAIT_"))
            {
                var waitSecondsStr = rpcEx.Message.Replace("FLOOD_WAIT_", "");
                if (int.TryParse(waitSecondsStr, out var waitSeconds))
                {
                    logger.LogWarning(
                        "FLOOD_WAIT detected for channel {ChannelId}. Required wait: {Seconds} seconds",
                        channelId, waitSeconds);

                    if (waitSeconds <= 120)
                    {
                        logger.LogInformation("Waiting {Seconds} seconds before continuing...", waitSeconds + 2);
                        await Task.Delay(TimeSpan.FromSeconds(waitSeconds + 2));
                        
                        try
                        {
                            await client.Messages_ImportChatInvite(inviteLink.Replace("https://t.me/+", ""));
                        }
                        catch (RpcException retryEx) when (retryEx.Code == 400 && retryEx.Message is "USER_ALREADY_PARTICIPANT" or "INVITE_HASH_EXPIRED")
                        {
                            logger.LogInformation("Agent {AgentId} already in channel {ChannelId} after wait, skipping join", agent.Id, channel.Id);
                        }
                    }
                    else
                    {
                        channel.SetStatus(ChannelStatusType.UnReady);
                        channel.DetachAgent();
                        await dbContext.SaveChangesAsync();
                        
                        logger.LogWarning(
                            "Channel {ChannelId} marked as UnReady due to long FLOOD_WAIT ({Seconds}s). Background worker will retry later.",
                            channelId, waitSeconds);
                        
                        return Error.Failure("Agent.FloodWait", 
                            $"Telegram rate limit. Channel will be processed in background. Wait time: {waitSeconds}s");
                    }
                }
                else
                {
                    throw;
                }
            }

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
