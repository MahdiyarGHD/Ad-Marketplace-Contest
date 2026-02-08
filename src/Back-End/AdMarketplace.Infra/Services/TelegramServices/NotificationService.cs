using AdMarketplace.Database;
using AdMarketplace.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace AdMarketplace.Infra.Services.TelegramServices;

public class NotificationService(
    AdMarketDbContext dbContext,
    ITelegramBotClient botClient,
    ILogger<NotificationService> logger) : INotificationService
{
    public async Task NotifyDealCreatedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var text = $"📋 New deal created for your channel \"{deal.Channel.Title}\"\n" +
                   $"Amount: {deal.AmountTon} TON\n" +
                   $"Deal ID: {deal.Id}";

        await SendSafeAsync(ownerChatId, text, ct);
    }

    public async Task NotifyEscrowFundedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var text = $"💰 Escrow funded for deal on \"{deal.Channel.Title}\"\n" +
                   $"Amount: {deal.AmountTon} TON\n" +
                   $"Please submit your ad draft.\n" +
                   $"Deal ID: {deal.Id}";

        await SendSafeAsync(ownerChatId, text, ct);
    }

    public async Task NotifyDraftSubmittedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var advertiserChatId = deal.Advertiser.UserId;
        var ownerChatId = deal.Channel.Owner.UserId;
        var text = $"📝 Draft submitted for review on \"{deal.Channel.Title}\"\n" +
                   $"Please review and approve or reject.\n" +
                   $"Deal ID: {deal.Id}";

        await SendSafeAsync(advertiserChatId, text, ct);

        if (deal.DraftMessageId.HasValue)
            await CopyMessageSafeAsync(advertiserChatId, ownerChatId, (int)deal.DraftMessageId.Value, ct);
    }

    public async Task NotifyDraftApprovedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var text = $"✅ Draft approved for \"{deal.Channel.Title}\"\n" +
                   $"The ad will be posted automatically.\n" +
                   $"Deal ID: {deal.Id}";

        await SendSafeAsync(ownerChatId, text, ct);
    }

    public async Task NotifyDraftRejectedAsync(Guid dealId, string feedback, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var text = $"❌ Draft rejected for \"{deal.Channel.Title}\"\n" +
                   $"Feedback: {feedback}\n" +
                   $"Please submit a revised draft.\n" +
                   $"Deal ID: {deal.Id}";

        await SendSafeAsync(ownerChatId, text, ct);
    }

    public async Task NotifyDealPostedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var advertiserChatId = deal.Advertiser.UserId;
        var text = $"📢 Your ad has been posted on \"{deal.Channel.Title}\"\n" +
                   $"Verification in progress.\n" +
                   $"Deal ID: {deal.Id}";

        await SendSafeAsync(advertiserChatId, text, ct);
    }

    public async Task NotifyDealCompletedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;

        await SendSafeAsync(ownerChatId,
            $"🎉 Deal completed! Funds released for \"{deal.Channel.Title}\"\n" +
            $"Amount: {deal.AmountTon} TON\nDeal ID: {deal.Id}", ct);

        await SendSafeAsync(advertiserChatId,
            $"🎉 Deal completed on \"{deal.Channel.Title}\"\n" +
            $"Deal ID: {deal.Id}", ct);
    }

    public async Task NotifyDealRefundedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;

        await SendSafeAsync(ownerChatId,
            $"🔄 Deal refunded for \"{deal.Channel.Title}\"\nDeal ID: {deal.Id}", ct);

        await SendSafeAsync(advertiserChatId,
            $"🔄 Refund processed for deal on \"{deal.Channel.Title}\"\n" +
            $"Amount: {deal.AmountTon} TON\nDeal ID: {deal.Id}", ct);
    }

    public async Task NotifyDealCancelledAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;

        await SendSafeAsync(ownerChatId,
            $"🚫 Deal cancelled for \"{deal.Channel.Title}\"\nDeal ID: {deal.Id}", ct);

        await SendSafeAsync(advertiserChatId,
            $"🚫 Deal cancelled on \"{deal.Channel.Title}\"\nDeal ID: {deal.Id}", ct);
    }

    public async Task NotifyDealDisputedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;

        await SendSafeAsync(ownerChatId,
            $"⚠️ Deal disputed for \"{deal.Channel.Title}\"\n" +
            $"The post may have been deleted or edited.\nDeal ID: {deal.Id}", ct);

        await SendSafeAsync(advertiserChatId,
            $"⚠️ Deal disputed on \"{deal.Channel.Title}\"\n" +
            $"The post may have been deleted or edited.\nDeal ID: {deal.Id}", ct);
    }

    private async Task<Database.Models.Deal?> LoadDealAsync(Guid dealId, CancellationToken ct)
    {
        var deal = await dbContext.Deals
            .Include(d => d.Channel)
                .ThenInclude(c => c.Owner)
            .Include(d => d.Advertiser)
            .FirstOrDefaultAsync(d => d.Id == dealId, ct);

        if (deal is null)
            logger.LogWarning("Notification skipped: deal {DealId} not found", dealId);

        return deal;
    }

    private async Task SendSafeAsync(long chatId, string text, CancellationToken ct)
    {
        try
        {
            await botClient.SendMessage(chatId, text, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send notification to chat {ChatId}", chatId);
        }
    }

    private async Task CopyMessageSafeAsync(long toChatId, long fromChatId, int messageId, CancellationToken ct)
    {
        try
        {
            await botClient.CopyMessage(toChatId, fromChatId, messageId, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to copy draft message {MessageId} to chat {ChatId}", messageId, toChatId);
        }
    }
}
