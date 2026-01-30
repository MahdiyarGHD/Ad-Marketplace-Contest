using AdMarketplace.Infra.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdMarketplace.Bot.Handlers;

public class UserChannelConnectionHandler(IUserChannelConnectionService userChannelConnectionService, ILogger<UserChannelConnectionHandler> logger) : IHandler
{
    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Type is not UpdateType.MyChatMember || update.MyChatMember?.Chat.Type is not ChatType.Channel)
            return;

        var chatMemberUpdate = update.MyChatMember;
        var newStatus = chatMemberUpdate.NewChatMember;
        
        switch (newStatus)
        {
            case ChatMemberLeft:
                await HandleChannelDisconnectAsync(chatMemberUpdate.Chat.Id);
                return;
            case ChatMemberAdministrator { CanInviteUsers: true, CanPromoteMembers: true }:
                await HandleChannelConnectAsync(
                    chatMemberUpdate.Chat.Title ?? string.Empty,
                    chatMemberUpdate.Chat.Id,
                    chatMemberUpdate.From.Id);
                break;
        }
    }

    private async Task HandleChannelConnectAsync(string title, long chatId, long userId)
    {
        var result = await userChannelConnectionService.CreateOrTouchAsync(title, userId, chatId);
        
        if (result.IsError)
            logger.LogError("Failed to connect channel {ChatId} for user {UserId}", chatId, userId);
    }

    private async Task HandleChannelDisconnectAsync(long chatId)
    {
        var result = await userChannelConnectionService.RemoveChannelConnectionsAsync(chatId);

        if (result.IsError)
            logger.LogError("Failed to remove connections for channel {ChatId}", chatId);
    }
}