using AdMarketplace.Domain.Options;
using AdMarketplace.Infra.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AdMarketplace.Bot.Handlers;

public class StartCommandHandler(
    ITelegramBotClient botClient,
    IOptions<TelegramBotOptions> botOptions,
    ILogger<StartCommandHandler> logger) : IHandler
{
    private readonly TelegramBotOptions _botOptions = botOptions.Value;

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update is not { Type: UpdateType.Message, Message: { Chat.Type: ChatType.Private } message })
            return;

        if (message.From is null)
            return;

        if (message.Text is not { } text || !text.StartsWith("/start"))
            return;

        await HandleStartCommandAsync(message, cancellationToken);
    }

    private async Task HandleStartCommandAsync(Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var firstName = message.From?.FirstName ?? "there";

        var welcomeText = $"""
            🎯 **Welcome to AdMarketplace, {firstName}!**
            
            Your gateway to premium Telegram channel advertising.
            
            ✨ **What you can do:**
            • 📊 Browse verified channels with real-time stats
            • 💎 Discover premium advertising opportunities
            • 🎬 Create and manage ad campaigns
            • 📈 Track campaign performance
            • 💰 Secure payments with TON blockchain
            
            Ready to grow your reach? Tap below to get started! 👇
            """;

        var miniAppUrl = $"https://t.me/{_botOptions.BotUsername}/{_botOptions.AppShortName}";

        var keyboard = new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithUrl(
                    "🚀 Open AdMarketplace",
                    miniAppUrl)
            ]
        ]);

        try
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: welcomeText,
                parseMode: ParseMode.Markdown,
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);

            logger.LogInformation("Sent start message to user {UserId}", chatId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send start message to user {UserId}", chatId);
        }
    }
}

