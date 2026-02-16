using AdMarketplace.Database;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AdMarketplace.Infra.Services.ChannelServices;

public class ChannelVerificationService(
    ITelegramBotClient botClient,
    AdMarketDbContext dbContext,
    ILogger<ChannelVerificationService> logger) : IChannelVerificationService
{
    public async Task<ErrorOr<bool>> VerifyBotAdminRightsAsync(long chatId, CancellationToken cancellationToken = default)
    {
        try
        {
            var me = await botClient.GetMe(cancellationToken);
            var botMember = await botClient.GetChatMember(chatId, me.Id, cancellationToken);

            var hasRequiredPermissions = botMember is ChatMemberAdministrator admin
                && admin.CanPromoteMembers
                && admin.CanInviteUsers
                && admin.CanDeleteMessages
                && admin.CanPostMessages
                && admin.CanEditMessages;

            if (!hasRequiredPermissions)
            {
                logger.LogWarning("Bot does not have required admin rights in channel {ChatId}", chatId);
                return Error.Forbidden("Channel.BotNotAdmin", 
                    "The bot does not have the required admin permissions in the channel");
            }

            return true;
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException ex)
        {
            logger.LogError(ex, "Failed to verify bot permissions in channel {ChatId}", chatId);
            return Error.Failure("Channel.VerificationFailed", 
                $"Failed to verify bot permissions: {ex.Message}");
        }
    }

    public async Task<ErrorOr<bool>> VerifyAgentAdminRightsAsync(long chatId, long agentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            var member = await botClient.GetChatMember(chatId, agentUserId, cancellationToken);

            var hasRequiredPermissions = member is ChatMemberAdministrator { CanPostMessages: true, CanEditMessages: true } and { CanDeleteMessages: true, CanInviteUsers: true, CanPinMessages: true };

            if (!hasRequiredPermissions)
            {
                logger.LogWarning("Agent {AgentUserId} does not have required admin rights in channel {ChatId}", 
                    agentUserId, chatId);
                return Error.Forbidden("Channel.AgentNotAdmin", 
                    "The agent does not have the required admin permissions in the channel");
            }

            return true;
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException ex)
        {
            logger.LogError(ex, "Failed to verify agent permissions in channel {ChatId}", chatId);
            return Error.Failure("Channel.VerificationFailed", 
                $"Failed to verify agent permissions: {ex.Message}");
        }
    }

    public async Task<ErrorOr<bool>> VerifyChannelReadinessAsync(Guid channelId, CancellationToken cancellationToken = default)
    {
        var channel = await dbContext.Channels
            .Include(c => c.Agent)
            .FirstOrDefaultAsync(c => c.Id == channelId, cancellationToken);

        if (channel is null)
        {
            return Error.NotFound("Channel.NotFound", "Channel not found");
        }

        var botVerification = await VerifyBotAdminRightsAsync(channel.ChatId, cancellationToken);
        if (botVerification.IsError)
        {
            return botVerification.Errors;
        }

        if (channel is { AgentId: not null, Agent: not null })
        {
            var agentVerification = await VerifyAgentAdminRightsAsync(channel.ChatId, channel.Agent.UserId, cancellationToken);
            if (agentVerification.IsError)
            {
                return agentVerification.Errors;
            }
        }

        return true;
    }
}

