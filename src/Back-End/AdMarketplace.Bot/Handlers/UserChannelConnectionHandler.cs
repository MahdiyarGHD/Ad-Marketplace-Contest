using AdMarketplace.Infra.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdMarketplace.Bot.Handlers;

public class UserChannelConnectionHandler(IUserChannelConnectionService userChannelConnectionService, ILogger<UserChannelConnectionHandler> logger) : IHandler
{
    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Type is not UpdateType.MyChatMember)
            return;

        if (update.MyChatMember?.Chat.Type is not ChatType.Channel)
            return;

        if (update.MyChatMember?.NewChatMember is ChatMemberLeft or 
            not ChatMemberAdministrator 
                { CanInviteUsers: true, CanPromoteMembers: true})
            return;
        
        await userChannelConnectionService
            .CreateOrTouchAsync(
                title: update.MyChatMember!.Chat.Title ?? string.Empty,
                chatId: update.MyChatMember!.Chat.Id,
                userId: update.MyChatMember!.From.Id
            );
    }
}