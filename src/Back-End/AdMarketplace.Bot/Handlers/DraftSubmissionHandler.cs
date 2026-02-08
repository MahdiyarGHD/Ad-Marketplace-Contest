using AdMarketplace.Database;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AdMarketplace.Bot.Handlers;

public class DraftSubmissionHandler(
    AdMarketDbContext dbContext,
    IDealService dealService,
    IUserService userService,
    ITelegramBotClient botClient,
    ILogger<DraftSubmissionHandler> logger) : IHandler
{
    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update is not { Type: UpdateType.Message, Message: { Chat.Type: ChatType.Private } message })
            return;

        if (message.From is null)
            return;

        if (message.Text is not null && message.Text.StartsWith('/'))
            return;

        var telegramUserId = message.From.Id;

        var user = await userService.GetByUserIdAsync(telegramUserId);
        if (user.IsError)
            return;

        var pendingDeals = await dbContext.Deals
            .Include(d => d.Channel)
            .Where(d => d.Channel.OwnerId == user.Value.Id)
            .Where(d => d.Status == DealStatusType.EscrowFunded || d.Status == DealStatusType.DraftRejected)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);

        if (pendingDeals.Count == 0)
            return;

        if (pendingDeals.Count == 1)
        {
            var deal = pendingDeals[0];
            var result = await dealService.SubmitDraftAsync(deal.Id, user.Value.Id, message.MessageId);
            if (result.IsError)
            {
                logger.LogWarning("Failed to submit draft for deal {DealId}: {Error}",
                    deal.Id, result.FirstError.Description);
                return;
            }

            await botClient.SendMessage(
                telegramUserId,
                $"✅ Draft submitted for \"{deal.Channel.Title}\".\nThe advertiser will review it.",
                cancellationToken: cancellationToken);
            return;
        }

        var buttons = pendingDeals.Select(d => new[]
        {
            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                d.Channel.Title, $"draft:{d.Id}:{message.MessageId}")
        });

        await botClient.SendMessage(
            telegramUserId,
            "You have multiple deals awaiting a draft. Which deal is this draft for?",
            replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons),
            cancellationToken: cancellationToken);
    }

    public async Task HandleCallbackAsync(Update update, CancellationToken cancellationToken)
    {
        if (update is not { Type: UpdateType.CallbackQuery, CallbackQuery: { Data: { } data } callback })
            return;

        if (!data.StartsWith("draft:"))
            return;

        var parts = data.Split(':');
        if (parts.Length != 3 || !Guid.TryParse(parts[1], out var dealId) || !int.TryParse(parts[2], out var messageId))
            return;

        var telegramUserId = callback.From.Id;
        var user = await userService.GetByUserIdAsync(telegramUserId);
        if (user.IsError)
            return;

        var result = await dealService.SubmitDraftAsync(dealId, user.Value.Id, messageId);
        if (result.IsError)
        {
            await botClient.AnswerCallbackQuery(callback.Id,
                "Failed to submit draft.", cancellationToken: cancellationToken);
            return;
        }

        await botClient.AnswerCallbackQuery(callback.Id,
            "Draft submitted!", cancellationToken: cancellationToken);

        if (callback.Message is not null)
        {
            await botClient.DeleteMessage(telegramUserId, callback.Message.MessageId,
                cancellationToken: cancellationToken);
        }

        var deal = result.Value;
        await botClient.SendMessage(
            telegramUserId,
            $"✅ Draft submitted for \"{deal.Channel.Title}\".\nThe advertiser will review it.",
            cancellationToken: cancellationToken);
    }
}
