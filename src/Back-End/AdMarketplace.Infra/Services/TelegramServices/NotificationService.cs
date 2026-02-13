using AdMarketplace.Database;
using AdMarketplace.Domain.Options;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace AdMarketplace.Infra.Services.TelegramServices;

public class NotificationService(
    AdMarketDbContext dbContext,
    ITelegramBotClient botClient,
    IOptions<TelegramBotOptions> botOptions,
    ILogger<NotificationService> logger) : INotificationService
{
    private readonly TelegramBotOptions _botOptions = botOptions.Value;

    private string CreateDeepLink(string type, Guid id)
    {
        var param = $"{type}_{id}";
        return $"https://t.me/{_botOptions.BotUsername}/{_botOptions.AppShortName}?startapp={param}";
    }

    private InlineKeyboardMarkup CreateDealButton(Guid dealId)
    {
        return new InlineKeyboardMarkup(
            InlineKeyboardButton.WithUrl("📋 View Deal", CreateDeepLink("deal", dealId))
        );
    }

    private InlineKeyboardMarkup CreateApplicationButton(Guid applicationId)
    {
        return new InlineKeyboardMarkup(
            InlineKeyboardButton.WithUrl("📝 View Application", CreateDeepLink("application", applicationId))
        );
    }

    private InlineKeyboardMarkup CreateInvitationButton(Guid invitationId)
    {
        return new InlineKeyboardMarkup(
            InlineKeyboardButton.WithUrl("📨 View Invitation", CreateDeepLink("invitation", invitationId))
        );
    }
    public async Task NotifyDealCreatedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var text = $"📋 New deal created for your channel \"{deal.Channel.Title}\"\n" +
                   $"Amount: {deal.AmountTon} TON\n" +
                   $"Deal ID: {deal.Id}";

        await SendSafeAsync(ownerChatId, text, ct, CreateDealButton(deal.Id));
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

        await SendSafeAsync(ownerChatId, text, ct, CreateDealButton(deal.Id));
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

        await SendSafeAsync(advertiserChatId, text, ct, CreateDealButton(deal.Id));

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

        await SendSafeAsync(ownerChatId, text, ct, CreateDealButton(deal.Id));
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

        await SendSafeAsync(ownerChatId, text, ct, CreateDealButton(deal.Id));
    }

    public async Task NotifyDealPostedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var advertiserChatId = deal.Advertiser.UserId;
        var text = $"📢 Your ad has been posted on \"{deal.Channel.Title}\"\n" +
                   $"Verification in progress.\n" +
                   $"Deal ID: {deal.Id}";

        await SendSafeAsync(advertiserChatId, text, ct, CreateDealButton(deal.Id));
    }

    public async Task NotifyDealCompletedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;
        var button = CreateDealButton(deal.Id);

        await SendSafeAsync(ownerChatId,
            $"🎉 Deal completed! Funds released for \"{deal.Channel.Title}\"\n" +
            $"Amount: {deal.AmountTon} TON\nDeal ID: {deal.Id}", ct, button);

        await SendSafeAsync(advertiserChatId,
            $"🎉 Deal completed on \"{deal.Channel.Title}\"\n" +
            $"Deal ID: {deal.Id}", ct, button);
    }

    public async Task NotifyDealRefundedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;
        var button = CreateDealButton(deal.Id);

        await SendSafeAsync(ownerChatId,
            $"🔄 Deal refunded for \"{deal.Channel.Title}\"\nDeal ID: {deal.Id}", ct, button);

        await SendSafeAsync(advertiserChatId,
            $"🔄 Refund processed for deal on \"{deal.Channel.Title}\"\n" +
            $"Amount: {deal.AmountTon} TON\nDeal ID: {deal.Id}", ct, button);
    }

    public async Task NotifyDealCancelledAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;
        var button = CreateDealButton(deal.Id);

        await SendSafeAsync(ownerChatId,
            $"🚫 Deal cancelled for \"{deal.Channel.Title}\"\nDeal ID: {deal.Id}", ct, button);

        await SendSafeAsync(advertiserChatId,
            $"🚫 Deal cancelled on \"{deal.Channel.Title}\"\nDeal ID: {deal.Id}", ct, button);
    }

    public async Task NotifyDealDisputedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;
        var button = CreateDealButton(deal.Id);

        await SendSafeAsync(ownerChatId,
            $"⚠️ Deal disputed for \"{deal.Channel.Title}\"\n" +
            $"The post may have been deleted or edited.\nDeal ID: {deal.Id}", ct, button);

        await SendSafeAsync(advertiserChatId,
            $"⚠️ Deal disputed on \"{deal.Channel.Title}\"\n" +
            $"The post may have been deleted or edited.\nDeal ID: {deal.Id}", ct, button);
    }

    public async Task NotifyDisputeResolvedAsync(Guid dealId, DisputeResolutionType resolution, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;
        var button = CreateDealButton(deal.Id);

        var resolutionText = resolution == DisputeResolutionType.RefundAdvertiser
            ? "Funds have been refunded to the advertiser."
            : "Funds have been released to the channel owner.";

        await SendSafeAsync(ownerChatId,
            $"⚖️ Dispute resolved for \"{deal.Channel.Title}\"\n" +
            $"{resolutionText}\nDeal ID: {deal.Id}", ct, button);

        await SendSafeAsync(advertiserChatId,
            $"⚖️ Dispute resolved on \"{deal.Channel.Title}\"\n" +
            $"{resolutionText}\nDeal ID: {deal.Id}", ct, button);
    }

    public async Task NotifyChannelApplicationReceivedAsync(Guid applicationId, CancellationToken ct = default)
    {
        var app = await LoadChannelApplicationAsync(applicationId, ct);
        if (app is null) return;

        var ownerChatId = app.Channel.Owner.UserId;
        await SendSafeAsync(ownerChatId,
            $"📩 New application for your channel \"{app.Channel.Title}\"\n" +
            $"From: {app.Advertiser.FirstName}\n" +
            $"Format: {app.ProposedAdFormat}, Price: {app.ProposedPriceTon} TON ({app.ProposedPriceType})\n" +
            (app.Message is not null ? $"Message: {app.Message}\n" : "") +
            $"Application ID: {app.Id}", ct, CreateApplicationButton(app.Id));
    }

    public async Task NotifyChannelApplicationAcceptedAsync(Guid applicationId, CancellationToken ct = default)
    {
        var app = await LoadChannelApplicationAsync(applicationId, ct);
        if (app is null) return;

        var advertiserChatId = app.Advertiser.UserId;
        await SendSafeAsync(advertiserChatId,
            $"✅ Your application for \"{app.Channel.Title}\" has been accepted!\n" +
            $"Format: {app.ProposedAdFormat}, Price: {app.ProposedPriceTon} TON\n" +
            $"A deal has been created automatically. Please fund the escrow.\n" +
            $"Application ID: {app.Id}", ct, CreateApplicationButton(app.Id));
    }

    public async Task NotifyChannelApplicationRejectedAsync(Guid applicationId, string? reason, CancellationToken ct = default)
    {
        var app = await LoadChannelApplicationAsync(applicationId, ct);
        if (app is null) return;

        var advertiserChatId = app.Advertiser.UserId;
        await SendSafeAsync(advertiserChatId,
            $"❌ Your application for \"{app.Channel.Title}\" has been rejected.\n" +
            (reason is not null ? $"Reason: {reason}\n" : "") +
            $"Application ID: {app.Id}", ct, CreateApplicationButton(app.Id));
    }

    public async Task NotifyChannelApplicationCounterOfferAsync(Guid applicationId, CancellationToken ct = default)
    {
        var app = await LoadChannelApplicationAsync(applicationId, ct);
        if (app is null) return;

        var recipientChatId = app.LastCounterByUserId == app.AdvertiserId
            ? app.Channel.Owner.UserId
            : app.Advertiser.UserId;

        await SendSafeAsync(recipientChatId,
            $"🔄 Counter-offer on \"{app.Channel.Title}\"\n" +
            $"New terms: {app.CounterAdFormat}, {app.CounterPriceTon} TON ({app.CounterPriceType})\n" +
            (app.CounterMessage is not null ? $"Message: {app.CounterMessage}\n" : "") +
            $"You can accept, reject, or counter.\n" +
            $"Application ID: {app.Id}", ct, CreateApplicationButton(app.Id));
    }

    public async Task NotifyCampaignApplicationReceivedAsync(Guid applicationId, CancellationToken ct = default)
    {
        var app = await LoadCampaignApplicationAsync(applicationId, ct);
        if (app is null) return;

        var advertiserChatId = app.Campaign.Advertiser.UserId;
        await SendSafeAsync(advertiserChatId,
            $"📩 New application for your campaign \"{app.Campaign.Title}\"\n" +
            $"Channel: {app.Channel.Title}\n" +
            $"Format: {app.ProposedAdFormat}, Price: {app.ProposedPriceTon} TON ({app.ProposedPriceType})\n" +
            (app.Message is not null ? $"Message: {app.Message}\n" : "") +
            $"Application ID: {app.Id}", ct);
    }

    public async Task NotifyCampaignApplicationAcceptedAsync(Guid applicationId, CancellationToken ct = default)
    {
        var app = await LoadCampaignApplicationAsync(applicationId, ct);
        if (app is null) return;

        var ownerChatId = app.Channel.Owner.UserId;
        await SendSafeAsync(ownerChatId,
            $"✅ Your application for campaign \"{app.Campaign.Title}\" has been accepted!\n" +
            $"Channel: {app.Channel.Title}\n" +
            $"Format: {app.ProposedAdFormat}, Price: {app.ProposedPriceTon} TON\n" +
            $"A deal has been created automatically.\n" +
            $"Application ID: {app.Id}", ct);
    }

    public async Task NotifyCampaignApplicationRejectedAsync(Guid applicationId, string? reason, CancellationToken ct = default)
    {
        var app = await LoadCampaignApplicationAsync(applicationId, ct);
        if (app is null) return;

        var ownerChatId = app.Channel.Owner.UserId;
        await SendSafeAsync(ownerChatId,
            $"❌ Your application for campaign \"{app.Campaign.Title}\" has been rejected.\n" +
            (reason is not null ? $"Reason: {reason}\n" : "") +
            $"Application ID: {app.Id}", ct);
    }

    public async Task NotifyCampaignApplicationCounterOfferAsync(Guid applicationId, CancellationToken ct = default)
    {
        var app = await LoadCampaignApplicationAsync(applicationId, ct);
        if (app is null) return;

        var lastCounterByAdvertiser = app.LastCounterByUserId == app.Campaign.AdvertiserId;
        var recipientChatId = lastCounterByAdvertiser
            ? app.Channel.Owner.UserId
            : app.Campaign.Advertiser.UserId;

        await SendSafeAsync(recipientChatId,
            $"🔄 Counter-offer on campaign \"{app.Campaign.Title}\"\n" +
            $"Channel: {app.Channel.Title}\n" +
            $"New terms: {app.CounterAdFormat}, {app.CounterPriceTon} TON ({app.CounterPriceType})\n" +
            (app.CounterMessage is not null ? $"Message: {app.CounterMessage}\n" : "") +
            $"You can accept, reject, or counter.\n" +
            $"Application ID: {app.Id}", ct);
    }

    public async Task NotifyCampaignInvitationReceivedAsync(Guid invitationId, CancellationToken ct = default)
    {
        var invitation = await LoadCampaignInvitationAsync(invitationId, ct);
        if (invitation is null) return;

        var ownerChatId = invitation.Channel.Owner.UserId;
        await SendSafeAsync(ownerChatId,
            $"📨 You've been invited to campaign \"{invitation.Campaign.Title}\"\n" +
            $"Channel: {invitation.Channel.Title}\n" +
            $"Format: {invitation.ProposedAdFormat}, Price: {invitation.ProposedPriceTon} TON ({invitation.ProposedPriceType})\n" +
            (invitation.Message is not null ? $"Message: {invitation.Message}\n" : "") +
            $"Invitation ID: {invitation.Id}", ct);
    }

    public async Task NotifyCampaignInvitationAcceptedAsync(Guid invitationId, CancellationToken ct = default)
    {
        var invitation = await LoadCampaignInvitationAsync(invitationId, ct);
        if (invitation is null) return;

        var advertiserChatId = invitation.Campaign.Advertiser.UserId;
        await SendSafeAsync(advertiserChatId,
            $"✅ Your invitation for \"{invitation.Channel.Title}\" has been accepted!\n" +
            $"Campaign: {invitation.Campaign.Title}\n" +
            $"Format: {invitation.ProposedAdFormat}, Price: {invitation.ProposedPriceTon} TON\n" +
            $"A deal has been created automatically. Please fund the escrow.\n" +
            $"Invitation ID: {invitation.Id}", ct);
    }

    public async Task NotifyCampaignInvitationRejectedAsync(Guid invitationId, string? reason, CancellationToken ct = default)
    {
        var invitation = await LoadCampaignInvitationAsync(invitationId, ct);
        if (invitation is null) return;

        var advertiserChatId = invitation.Campaign.Advertiser.UserId;
        await SendSafeAsync(advertiserChatId,
            $"❌ Your invitation to \"{invitation.Channel.Title}\" has been rejected.\n" +
            $"Campaign: {invitation.Campaign.Title}\n" +
            (reason is not null ? $"Reason: {reason}\n" : "") +
            $"Invitation ID: {invitation.Id}", ct);
    }

    public async Task NotifyCampaignInvitationWithdrawnAsync(Guid invitationId, CancellationToken ct = default)
    {
        var invitation = await LoadCampaignInvitationAsync(invitationId, ct);
        if (invitation is null) return;

        var ownerChatId = invitation.Channel.Owner.UserId;
        await SendSafeAsync(ownerChatId,
            $"🔙 Invitation withdrawn for campaign \"{invitation.Campaign.Title}\"\n" +
            $"Channel: {invitation.Channel.Title}\n" +
            $"Invitation ID: {invitation.Id}", ct, CreateInvitationButton(invitation.Id));
    }

    public async Task NotifyPostDeletedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;
        var button = CreateDealButton(deal.Id);

        var durationText = deal.RequiredPostDurationHours.HasValue
            ? $"{deal.RequiredPostDurationHours.Value:F1} hours"
            : "the agreed duration";

        await SendSafeAsync(ownerChatId,
            $"🗑️ Ad post removed from \"{deal.Channel.Title}\"\n" +
            $"The post duration of {durationText} has ended.\n" +
            $"Deal ID: {deal.Id}", ct, button);

        await SendSafeAsync(advertiserChatId,
            $"🗑️ Your ad has been removed from \"{deal.Channel.Title}\"\n" +
            $"The post duration of {durationText} has ended.\n" +
            $"Deal ID: {deal.Id}", ct, button);
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

    private async Task<Database.Models.ChannelApplication?> LoadChannelApplicationAsync(Guid applicationId, CancellationToken ct)
    {
        var app = await dbContext.ChannelApplications
            .Include(a => a.Channel)
                .ThenInclude(c => c.Owner)
            .Include(a => a.Advertiser)
            .FirstOrDefaultAsync(a => a.Id == applicationId, ct);

        if (app is null)
            logger.LogWarning("Notification skipped: channel application {AppId} not found", applicationId);

        return app;
    }

    private async Task<Database.Models.CampaignApplication?> LoadCampaignApplicationAsync(Guid applicationId, CancellationToken ct)
    {
        var app = await dbContext.CampaignApplications
            .Include(a => a.Campaign)
                .ThenInclude(c => c.Advertiser)
            .Include(a => a.Channel)
                .ThenInclude(c => c.Owner)
            .FirstOrDefaultAsync(a => a.Id == applicationId, ct);

        if (app is null)
            logger.LogWarning("Notification skipped: campaign application {AppId} not found", applicationId);

        return app;
    }

    private async Task<Database.Models.CampaignInvitation?> LoadCampaignInvitationAsync(Guid invitationId, CancellationToken ct)
    {
        var invitation = await dbContext.CampaignInvitations
            .Include(i => i.Campaign)
                .ThenInclude(c => c.Advertiser)
            .Include(i => i.Channel)
                .ThenInclude(c => c.Owner)
            .FirstOrDefaultAsync(i => i.Id == invitationId, ct);

        if (invitation is null)
            logger.LogWarning("Notification skipped: campaign invitation {InvitationId} not found", invitationId);

        return invitation;
    }

    private async Task SendSafeAsync(long chatId, string text, CancellationToken ct, InlineKeyboardMarkup? replyMarkup = null)
    {
        try
        {
            await botClient.SendMessage(chatId, text, replyMarkup: replyMarkup, cancellationToken: ct);
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
