using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdMarketplace.Bot.Extensions;

public static class UpdateHandlerExtensions
{
    
    extension(Update update)
    {
        public bool IsRelevantUpdate() =>
            update is { Type: UpdateType.Message, Message.Chat.Type: ChatType.Private } ||
            (update.Type == UpdateType.CallbackQuery && update.CallbackQuery?.Message?.Chat.Type == ChatType.Private) ||
            update is { Type: UpdateType.MyChatMember, MyChatMember.Chat.Type: ChatType.Channel };

        public User? GetUserFromUpdate()
        {
            var telegramUser = update switch
            {
                { Message.From: { } user } => user,
                { CallbackQuery.From: { } user } => user,
                { MyChatMember.From: { } user } => user,
                { ChatMember.From: { } user } => user,
                { InlineQuery.From: { } user } => user,
                _ => null
            };
        
            return telegramUser;
        }
    }
}