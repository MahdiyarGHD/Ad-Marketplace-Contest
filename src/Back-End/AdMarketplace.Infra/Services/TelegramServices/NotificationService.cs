using AdMarketplace.Database;
using AdMarketplace.Domain.Options;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
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

    private InlineKeyboardMarkup CreateCampaignApplicationButton(Guid applicationId)
    {
        return new InlineKeyboardMarkup(
            InlineKeyboardButton.WithUrl("📝 View Campaign Application", CreateDeepLink("campaign_application", applicationId))
        );
    }

    private InlineKeyboardMarkup CreateChannelApplicationButton(Guid applicationId)
    {
        return new InlineKeyboardMarkup(
            InlineKeyboardButton.WithUrl("📝 View Channel Application", CreateDeepLink("channel_application", applicationId))
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
        var scheduledInfo = deal.ScheduledPostTime.HasValue
            ? $"📅 Scheduled: {deal.ScheduledPostTime.Value:MMM dd, yyyy 'at' HH:mm} UTC\n"
            : "";
        var durationInfo = deal.RequiredPostDurationHours.HasValue
            ? $"⏱ Duration: {FormatDuration(deal.RequiredPostDurationHours.Value)}\n"
            : "";

        var text = $"📋 <b>New Deal Created</b>\n\n" +
                   $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
                   $"👤 Advertiser: {EscapeHtml(deal.Advertiser.FirstName)}\n" +
                   $"📝 Format: {FormatAdType(deal.AdFormat)}\n" +
                   $"💰 Amount: <b>{deal.AmountTon} TON</b> ({FormatPriceType(deal.PriceType)})\n" +
                   scheduledInfo +
                   durationInfo +
                   $"\n⏳ <i>Awaiting payment from advertiser...</i>";

        await SendSafeAsync(ownerChatId, text, ct, CreateDealButton(deal.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyEscrowFundedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var scheduledInfo = deal.ScheduledPostTime.HasValue
            ? $"📅 Posting scheduled for: {deal.ScheduledPostTime.Value:MMM dd, yyyy 'at' HH:mm} UTC\n"
            : "";

        var text = $"💰 <b>Escrow Funded!</b>\n\n" +
                   $"Great news! Payment has been secured for your channel.\n\n" +
                   $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
                   $"💵 Amount: <b>{deal.AmountTon} TON</b>\n" +
                   $"📝 Format: {FormatAdType(deal.AdFormat)}\n" +
                   scheduledInfo +
                   $"\n✏️ <b>Next Step:</b> Please submit your ad draft for review.";

        await SendSafeAsync(ownerChatId, text, ct, CreateDealButton(deal.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyDraftSubmittedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var advertiserChatId = deal.Advertiser.UserId;
        var ownerChatId = deal.Channel.Owner.UserId;
        var scheduledInfo = deal.ScheduledPostTime.HasValue
            ? $"📅 Scheduled for: {deal.ScheduledPostTime.Value:MMM dd, yyyy 'at' HH:mm} UTC\n"
            : "";

        var text = $"📝 <b>Draft Submitted for Review</b>\n\n" +
                   $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
                   $"📝 Format: {FormatAdType(deal.AdFormat)}\n" +
                   scheduledInfo +
                   $"\n👇 <i>The draft content is attached below.</i>\n\n" +
                   $"✅ Please review and <b>approve</b> or <b>reject</b> the draft.";

        await SendSafeAsync(advertiserChatId, text, ct, CreateDealButton(deal.Id), parseMode: ParseMode.Html);

        if (deal.DraftMessageId.HasValue)
            await CopyMessageSafeAsync(advertiserChatId, ownerChatId, (int)deal.DraftMessageId.Value, ct);
    }

    public async Task NotifyDraftApprovedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var scheduledInfo = deal.ScheduledPostTime.HasValue
            ? $"📅 Posting at: <b>{deal.ScheduledPostTime.Value:MMM dd, yyyy 'at' HH:mm} UTC</b>"
            : "📅 Posting: <b>As soon as possible</b>";

        var text = $"✅ <b>Draft Approved!</b>\n\n" +
                   $"Your ad draft has been approved by the advertiser.\n\n" +
                   $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
                   $"📝 Format: {FormatAdType(deal.AdFormat)}\n" +
                   $"💰 Amount: <b>{deal.AmountTon} TON</b>\n" +
                   scheduledInfo +
                   $"\n\n🚀 <i>The ad will be auto-posted at the scheduled time.</i>";

        await SendSafeAsync(ownerChatId, text, ct, CreateDealButton(deal.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyDraftRejectedAsync(Guid dealId, string feedback, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var text = $"❌ <b>Draft Rejected</b>\n\n" +
                   $"Your ad draft needs revision.\n\n" +
                   $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
                   $"📝 Format: {FormatAdType(deal.AdFormat)}\n\n" +
                   $"💬 <b>Feedback from advertiser:</b>\n" +
                   $"<i>\"{EscapeHtml(feedback)}\"</i>\n\n" +
                   $"✏️ Please submit a revised draft addressing the feedback.";

        await SendSafeAsync(ownerChatId, text, ct, CreateDealButton(deal.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyDealPostedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var advertiserChatId = deal.Advertiser.UserId;
        var ownerChatId = deal.Channel.Owner.UserId;
        var durationInfo = deal.RequiredPostDurationHours.HasValue
            ? $"⏱ Post duration: {FormatDuration(deal.RequiredPostDurationHours.Value)}\n"
            : "";
        var channelLink = deal.Channel.Username != null
            ? $"🔗 Channel: @{deal.Channel.Username}\n"
            : "";

        var advertiserText = $"📢 <b>Ad Posted!</b>\n\n" +
                   $"Your ad is now live!\n\n" +
                   $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
                   channelLink +
                   $"👥 Subscribers: {FormatNumber(deal.Channel.SubscriberCount)}\n" +
                   $"📝 Format: {FormatAdType(deal.AdFormat)}\n" +
                   durationInfo +
                   $"\n🔍 <i>Verification in progress to ensure the post stays up...</i>";

        var ownerText = $"📢 <b>Ad Posted Successfully!</b>\n\n" +
                   $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
                   $"💰 Amount: <b>{deal.AmountTon} TON</b>\n" +
                   durationInfo +
                   $"\n⏳ <i>Funds will be released after verification period.</i>";

        await SendSafeAsync(advertiserChatId, advertiserText, ct, CreateDealButton(deal.Id), parseMode: ParseMode.Html);
        await SendSafeAsync(ownerChatId, ownerText, ct, CreateDealButton(deal.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyDealCompletedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;
        var button = CreateDealButton(deal.Id);

        var ownerText = $"🎉 <b>Deal Completed!</b>\n\n" +
            $"Congratulations! Your payment has been released.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
            $"💵 Amount Received: <b>{deal.AmountTon} TON</b>\n" +
            $"📝 Format: {FormatAdType(deal.AdFormat)}\n\n" +
            $"✨ <i>Thank you for using AdMarketplace!</i>";

        var advertiserText = $"🎉 <b>Deal Completed!</b>\n\n" +
            $"Your ad campaign has been successfully completed.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
            $"👥 Reached: {FormatNumber(deal.Channel.SubscriberCount)} subscribers\n" +
            $"📝 Format: {FormatAdType(deal.AdFormat)}\n" +
            $"💰 Amount: <b>{deal.AmountTon} TON</b>\n\n" +
            $"✨ <i>Thank you for using AdMarketplace!</i>";

        await SendSafeAsync(ownerChatId, ownerText, ct, button, parseMode: ParseMode.Html);
        await SendSafeAsync(advertiserChatId, advertiserText, ct, button, parseMode: ParseMode.Html);
    }

    public async Task NotifyDealRefundedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;
        var button = CreateDealButton(deal.Id);

        var ownerText = $"🔄 <b>Deal Refunded</b>\n\n" +
            $"The deal has been refunded to the advertiser.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
            $"💰 Amount: {deal.AmountTon} TON\n" +
            $"📝 Format: {FormatAdType(deal.AdFormat)}\n\n" +
            $"<i>No funds have been charged.</i>";

        var advertiserText = $"🔄 <b>Refund Processed</b>\n\n" +
            $"Your payment has been refunded.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
            $"💵 Refunded: <b>{deal.AmountTon} TON</b>\n" +
            $"📝 Format: {FormatAdType(deal.AdFormat)}\n\n" +
            $"<i>Funds should arrive in your wallet shortly.</i>";

        await SendSafeAsync(ownerChatId, ownerText, ct, button, parseMode: ParseMode.Html);
        await SendSafeAsync(advertiserChatId, advertiserText, ct, button, parseMode: ParseMode.Html);
    }

    public async Task NotifyDealCancelledAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;
        var button = CreateDealButton(deal.Id);

        var ownerText = $"🚫 <b>Deal Cancelled</b>\n\n" +
            $"This deal has been cancelled due to inactivity.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
            $"📝 Format: {FormatAdType(deal.AdFormat)}\n" +
            $"💰 Amount: {deal.AmountTon} TON\n\n" +
            $"<i>The deal timed out without payment.</i>";

        var advertiserText = $"🚫 <b>Deal Cancelled</b>\n\n" +
            $"Your deal has been cancelled due to inactivity.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
            $"📝 Format: {FormatAdType(deal.AdFormat)}\n" +
            $"💰 Amount: {deal.AmountTon} TON\n\n" +
            $"<i>No payment was processed.</i>";

        await SendSafeAsync(ownerChatId, ownerText, ct, button, parseMode: ParseMode.Html);
        await SendSafeAsync(advertiserChatId, advertiserText, ct, button, parseMode: ParseMode.Html);
    }

    public async Task NotifyDealDisputedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;
        var button = CreateDealButton(deal.Id);

        var ownerText = $"⚠️ <b>Deal Disputed</b>\n\n" +
            $"An issue has been detected with your ad post.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
            $"📝 Format: {FormatAdType(deal.AdFormat)}\n" +
            $"💰 Amount: {deal.AmountTon} TON\n\n" +
            $"⚠️ <i>The post may have been deleted or edited. Our team will review this.</i>";

        var advertiserText = $"⚠️ <b>Deal Disputed</b>\n\n" +
            $"An issue has been detected with your ad.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
            $"📝 Format: {FormatAdType(deal.AdFormat)}\n" +
            $"💰 Amount: {deal.AmountTon} TON\n\n" +
            $"⚠️ <i>The post may have been deleted or edited. Our team will review this.</i>";

        await SendSafeAsync(ownerChatId, ownerText, ct, button, parseMode: ParseMode.Html);
        await SendSafeAsync(advertiserChatId, advertiserText, ct, button, parseMode: ParseMode.Html);
    }

    public async Task NotifyDisputeResolvedAsync(Guid dealId, DisputeResolutionType resolution, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;
        var button = CreateDealButton(deal.Id);

        var (resolutionText, ownerOutcome, advertiserOutcome) = resolution == DisputeResolutionType.RefundAdvertiser
            ? ("Refund to Advertiser", "Funds have been refunded to the advertiser.", "Your payment has been refunded.")
            : ("Release to Channel", "Funds have been released to you.", "Funds have been released to the channel owner.");

        var ownerText = $"⚖️ <b>Dispute Resolved</b>\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
            $"📝 Format: {FormatAdType(deal.AdFormat)}\n" +
            $"💰 Amount: {deal.AmountTon} TON\n\n" +
            $"📋 Resolution: <b>{resolutionText}</b>\n" +
            $"💵 {ownerOutcome}";

        var advertiserText = $"⚖️ <b>Dispute Resolved</b>\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
            $"📝 Format: {FormatAdType(deal.AdFormat)}\n" +
            $"💰 Amount: {deal.AmountTon} TON\n\n" +
            $"📋 Resolution: <b>{resolutionText}</b>\n" +
            $"💵 {advertiserOutcome}";

        await SendSafeAsync(ownerChatId, ownerText, ct, button, parseMode: ParseMode.Html);
        await SendSafeAsync(advertiserChatId, advertiserText, ct, button, parseMode: ParseMode.Html);
    }

    public async Task NotifyChannelApplicationReceivedAsync(Guid applicationId, CancellationToken ct = default)
    {
        var app = await LoadChannelApplicationAsync(applicationId, ct);
        if (app is null) return;

        var ownerChatId = app.Channel.Owner.UserId;
        var scheduledInfo = app.ProposedPostingTime.HasValue
            ? $"📅 Proposed posting: {app.ProposedPostingTime.Value:MMM dd, yyyy 'at' HH:mm} UTC\n"
            : "";
        var messageInfo = app.Message is not null
            ? $"\n💬 <b>Message:</b>\n<i>\"{EscapeHtml(app.Message)}\"</i>\n"
            : "";

        var text = $"📩 <b>New Application Received</b>\n\n" +
            $"An advertiser wants to place an ad on your channel!\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(app.Channel.Title)}</b>\n" +
            $"👤 Advertiser: {EscapeHtml(app.Advertiser.FirstName)}\n" +
            $"📝 Format: {FormatAdType(app.ProposedAdFormat)}\n" +
            $"💰 Offer: <b>{app.ProposedPriceTon} TON</b> ({FormatPriceType(app.ProposedPriceType)})\n" +
            scheduledInfo +
            messageInfo +
            $"\n✅ Review and respond to this application.";

        await SendSafeAsync(ownerChatId, text, ct, CreateChannelApplicationButton(app.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyChannelApplicationAcceptedAsync(Guid applicationId, CancellationToken ct = default)
    {
        var app = await LoadChannelApplicationAsync(applicationId, ct);
        if (app is null) return;

        var advertiserChatId = app.Advertiser.UserId;
        var text = $"✅ <b>Application Accepted!</b>\n\n" +
            $"Great news! Your application has been accepted.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(app.Channel.Title)}</b>\n" +
            $"👥 Subscribers: {FormatNumber(app.Channel.SubscriberCount)}\n" +
            $"📝 Format: {FormatAdType(app.ProposedAdFormat)}\n" +
            $"💰 Price: <b>{app.ProposedPriceTon} TON</b>\n\n" +
            $"💳 <b>Next Step:</b> Please fund the escrow to proceed.";

        await SendSafeAsync(advertiserChatId, text, ct, CreateChannelApplicationButton(app.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyChannelApplicationRejectedAsync(Guid applicationId, string? reason, CancellationToken ct = default)
    {
        var app = await LoadChannelApplicationAsync(applicationId, ct);
        if (app is null) return;

        var advertiserChatId = app.Advertiser.UserId;
        var reasonInfo = reason is not null
            ? $"\n💬 <b>Reason:</b>\n<i>\"{EscapeHtml(reason)}\"</i>\n"
            : "";

        var text = $"❌ <b>Application Rejected</b>\n\n" +
            $"Unfortunately, your application was not accepted.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(app.Channel.Title)}</b>\n" +
            $"📝 Format: {FormatAdType(app.ProposedAdFormat)}\n" +
            $"💰 Offer: {app.ProposedPriceTon} TON\n" +
            reasonInfo +
            $"\n<i>You can try applying to other channels.</i>";

        await SendSafeAsync(advertiserChatId, text, ct, CreateChannelApplicationButton(app.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyChannelApplicationCounterOfferAsync(Guid applicationId, CancellationToken ct = default)
    {
        var app = await LoadChannelApplicationAsync(applicationId, ct);
        if (app is null) return;

        var recipientChatId = app.LastCounterByUserId == app.AdvertiserId
            ? app.Channel.Owner.UserId
            : app.Advertiser.UserId;
        var fromAdvertiser = app.LastCounterByUserId == app.AdvertiserId;
        var counterparty = fromAdvertiser ? "advertiser" : "channel owner";
        var messageInfo = app.CounterMessage is not null
            ? $"\n💬 <b>Message:</b>\n<i>\"{EscapeHtml(app.CounterMessage)}\"</i>\n"
            : "";

        var text = $"🔄 <b>Counter-Offer Received</b>\n\n" +
            $"The {counterparty} has proposed new terms.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(app.Channel.Title)}</b>\n" +
            $"📝 Format: {FormatAdType(app.CounterAdFormat ?? app.ProposedAdFormat)}\n" +
            $"💰 New Price: <b>{app.CounterPriceTon} TON</b> ({FormatPriceType(app.CounterPriceType ?? app.ProposedPriceType)})\n" +
            messageInfo +
            $"\n✅ You can <b>accept</b>, <b>reject</b>, or send a <b>counter-offer</b>.";

        await SendSafeAsync(recipientChatId, text, ct, CreateChannelApplicationButton(app.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyCampaignApplicationReceivedAsync(Guid applicationId, CancellationToken ct = default)
    {
        var app = await LoadCampaignApplicationAsync(applicationId, ct);
        if (app is null) return;

        var advertiserChatId = app.Campaign.Advertiser.UserId;
        var messageInfo = app.Message is not null
            ? $"\n💬 <b>Message:</b>\n<i>\"{EscapeHtml(app.Message)}\"</i>\n"
            : "";

        var text = $"📩 <b>New Campaign Application</b>\n\n" +
            $"A channel wants to participate in your campaign!\n\n" +
            $"📢 Campaign: <b>{EscapeHtml(app.Campaign.Title)}</b>\n" +
            $"🏷 Channel: <b>{EscapeHtml(app.Channel.Title)}</b>\n" +
            $"👥 Subscribers: {FormatNumber(app.Channel.SubscriberCount)}\n" +
            $"👁 Avg. Views: {FormatNumber(app.Channel.AverageViews)}\n" +
            $"📝 Format: {FormatAdType(app.ProposedAdFormat)}\n" +
            $"💰 Asking: <b>{app.ProposedPriceTon} TON</b> ({FormatPriceType(app.ProposedPriceType)})\n" +
            messageInfo +
            $"\n✅ Review this application.";

        await SendSafeAsync(advertiserChatId, text, ct, CreateCampaignApplicationButton(app.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyCampaignApplicationAcceptedAsync(Guid applicationId, CancellationToken ct = default)
    {
        var app = await LoadCampaignApplicationAsync(applicationId, ct);
        if (app is null) return;

        var ownerChatId = app.Channel.Owner.UserId;
        var text = $"✅ <b>Campaign Application Accepted!</b>\n\n" +
            $"Your channel has been selected for the campaign!\n\n" +
            $"📢 Campaign: <b>{EscapeHtml(app.Campaign.Title)}</b>\n" +
            $"🏷 Channel: <b>{EscapeHtml(app.Channel.Title)}</b>\n" +
            $"📝 Format: {FormatAdType(app.ProposedAdFormat)}\n" +
            $"💰 Price: <b>{app.ProposedPriceTon} TON</b>\n\n" +
            $"⏳ <i>A deal has been created. Awaiting advertiser payment.</i>";

        await SendSafeAsync(ownerChatId, text, ct, CreateCampaignApplicationButton(app.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyCampaignApplicationRejectedAsync(Guid applicationId, string? reason, CancellationToken ct = default)
    {
        var app = await LoadCampaignApplicationAsync(applicationId, ct);
        if (app is null) return;

        var ownerChatId = app.Channel.Owner.UserId;
        var reasonInfo = reason is not null
            ? $"\n💬 <b>Reason:</b>\n<i>\"{EscapeHtml(reason)}\"</i>\n"
            : "";

        var text = $"❌ <b>Campaign Application Rejected</b>\n\n" +
            $"Your application was not selected.\n\n" +
            $"📢 Campaign: <b>{EscapeHtml(app.Campaign.Title)}</b>\n" +
            $"🏷 Channel: <b>{EscapeHtml(app.Channel.Title)}</b>\n" +
            reasonInfo +
            $"\n<i>Keep exploring other campaigns!</i>";

        await SendSafeAsync(ownerChatId, text, ct, CreateCampaignApplicationButton(app.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyCampaignApplicationCounterOfferAsync(Guid applicationId, CancellationToken ct = default)
    {
        var app = await LoadCampaignApplicationAsync(applicationId, ct);
        if (app is null) return;

        var lastCounterByAdvertiser = app.LastCounterByUserId == app.Campaign.AdvertiserId;
        var recipientChatId = lastCounterByAdvertiser
            ? app.Channel.Owner.UserId
            : app.Campaign.Advertiser.UserId;
        var counterparty = lastCounterByAdvertiser ? "advertiser" : "channel owner";
        var messageInfo = app.CounterMessage is not null
            ? $"\n💬 <b>Message:</b>\n<i>\"{EscapeHtml(app.CounterMessage)}\"</i>\n"
            : "";

        var text = $"🔄 <b>Counter-Offer Received</b>\n\n" +
            $"The {counterparty} has proposed new terms.\n\n" +
            $"📢 Campaign: <b>{EscapeHtml(app.Campaign.Title)}</b>\n" +
            $"🏷 Channel: <b>{EscapeHtml(app.Channel.Title)}</b>\n" +
            $"📝 Format: {FormatAdType(app.CounterAdFormat ?? app.ProposedAdFormat)}\n" +
            $"💰 New Price: <b>{app.CounterPriceTon} TON</b> ({FormatPriceType(app.CounterPriceType ?? app.ProposedPriceType)})\n" +
            messageInfo +
            $"\n✅ You can <b>accept</b>, <b>reject</b>, or send a <b>counter-offer</b>.";

        await SendSafeAsync(recipientChatId, text, ct, CreateCampaignApplicationButton(app.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyCampaignInvitationReceivedAsync(Guid invitationId, CancellationToken ct = default)
    {
        var invitation = await LoadCampaignInvitationAsync(invitationId, ct);
        if (invitation is null) return;

        var ownerChatId = invitation.Channel.Owner.UserId;
        var scheduledInfo = invitation.ProposedPostingTime.HasValue
            ? $"📅 Proposed posting: {invitation.ProposedPostingTime.Value:MMM dd, yyyy 'at' HH:mm} UTC\n"
            : "";
        var messageInfo = invitation.Message is not null
            ? $"\n💬 <b>Message:</b>\n<i>\"{EscapeHtml(invitation.Message)}\"</i>\n"
            : "";

        var text = $"📨 <b>Campaign Invitation</b>\n\n" +
            $"You've been invited to participate in a campaign!\n\n" +
            $"📢 Campaign: <b>{EscapeHtml(invitation.Campaign.Title)}</b>\n" +
            $"🏷 Channel: <b>{EscapeHtml(invitation.Channel.Title)}</b>\n" +
            $"📝 Format: {FormatAdType(invitation.ProposedAdFormat)}\n" +
            $"💰 Offer: <b>{invitation.ProposedPriceTon} TON</b> ({FormatPriceType(invitation.ProposedPriceType)})\n" +
            scheduledInfo +
            messageInfo +
            $"\n✅ Review and respond to this invitation.";

        await SendSafeAsync(ownerChatId, text, ct, CreateInvitationButton(invitation.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyCampaignInvitationAcceptedAsync(Guid invitationId, CancellationToken ct = default)
    {
        var invitation = await LoadCampaignInvitationAsync(invitationId, ct);
        if (invitation is null) return;

        var advertiserChatId = invitation.Campaign.Advertiser.UserId;
        var text = $"✅ <b>Invitation Accepted!</b>\n\n" +
            $"Great news! Your invitation has been accepted.\n\n" +
            $"📢 Campaign: <b>{EscapeHtml(invitation.Campaign.Title)}</b>\n" +
            $"🏷 Channel: <b>{EscapeHtml(invitation.Channel.Title)}</b>\n" +
            $"👥 Subscribers: {FormatNumber(invitation.Channel.SubscriberCount)}\n" +
            $"📝 Format: {FormatAdType(invitation.ProposedAdFormat)}\n" +
            $"💰 Price: <b>{invitation.ProposedPriceTon} TON</b>\n\n" +
            $"💳 <b>Next Step:</b> Please fund the escrow to proceed.";

        await SendSafeAsync(advertiserChatId, text, ct, CreateInvitationButton(invitation.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyCampaignInvitationRejectedAsync(Guid invitationId, string? reason, CancellationToken ct = default)
    {
        var invitation = await LoadCampaignInvitationAsync(invitationId, ct);
        if (invitation is null) return;

        var advertiserChatId = invitation.Campaign.Advertiser.UserId;
        var reasonInfo = reason is not null
            ? $"\n💬 <b>Reason:</b>\n<i>\"{EscapeHtml(reason)}\"</i>\n"
            : "";

        var text = $"❌ <b>Invitation Declined</b>\n\n" +
            $"The channel owner has declined your invitation.\n\n" +
            $"📢 Campaign: <b>{EscapeHtml(invitation.Campaign.Title)}</b>\n" +
            $"🏷 Channel: <b>{EscapeHtml(invitation.Channel.Title)}</b>\n" +
            reasonInfo +
            $"\n<i>Consider inviting other channels.</i>";

        await SendSafeAsync(advertiserChatId, text, ct, CreateInvitationButton(invitation.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyCampaignInvitationWithdrawnAsync(Guid invitationId, CancellationToken ct = default)
    {
        var invitation = await LoadCampaignInvitationAsync(invitationId, ct);
        if (invitation is null) return;

        var ownerChatId = invitation.Channel.Owner.UserId;
        var text = $"🔙 <b>Invitation Withdrawn</b>\n\n" +
            $"The advertiser has withdrawn their invitation.\n\n" +
            $"📢 Campaign: <b>{EscapeHtml(invitation.Campaign.Title)}</b>\n" +
            $"🏷 Channel: <b>{EscapeHtml(invitation.Channel.Title)}</b>\n\n" +
            $"<i>This invitation is no longer available.</i>";

        await SendSafeAsync(ownerChatId, text, ct, CreateInvitationButton(invitation.Id), parseMode: ParseMode.Html);
    }

    public async Task NotifyPostDeletedAsync(Guid dealId, CancellationToken ct = default)
    {
        var deal = await LoadDealAsync(dealId, ct);
        if (deal is null) return;

        var ownerChatId = deal.Channel.Owner.UserId;
        var advertiserChatId = deal.Advertiser.UserId;
        var button = CreateDealButton(deal.Id);

        var durationText = deal.RequiredPostDurationHours.HasValue
            ? FormatDuration(deal.RequiredPostDurationHours.Value)
            : "the agreed duration";

        var ownerText = $"🗑️ <b>Ad Post Removed</b>\n\n" +
            $"The ad post has been removed as the duration period ended.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
            $"⏱ Duration: {durationText}\n" +
            $"📝 Format: {FormatAdType(deal.AdFormat)}\n\n" +
            $"✅ <i>Deal completed successfully!</i>";

        var advertiserText = $"🗑️ <b>Ad Post Removed</b>\n\n" +
            $"Your ad has been removed as the posting period ended.\n\n" +
            $"🏷 Channel: <b>{EscapeHtml(deal.Channel.Title)}</b>\n" +
            $"⏱ Duration: {durationText}\n" +
            $"📝 Format: {FormatAdType(deal.AdFormat)}\n\n" +
            $"✅ <i>Campaign completed successfully!</i>";

        await SendSafeAsync(ownerChatId, ownerText, ct, button, parseMode: ParseMode.Html);
        await SendSafeAsync(advertiserChatId, advertiserText, ct, button, parseMode: ParseMode.Html);
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

    private async Task SendSafeAsync(long chatId, string text, CancellationToken ct, InlineKeyboardMarkup? replyMarkup = null, ParseMode parseMode = ParseMode.Html)
    {
        try
        {
            await botClient.SendMessage(chatId, text, replyMarkup: replyMarkup, parseMode: parseMode, cancellationToken: ct);
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

    private static string EscapeHtml(string text) =>
        text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    private static string FormatAdType(AdFormatType type) => type switch
    {
        AdFormatType.Post => "📄 Post",
        _ => type.ToString()
    };

    private static string FormatPriceType(PriceType type) => type switch
    {
        PriceType.PerHour => "per hour",
        PriceType.PerDay => "per day",
        PriceType.PerThousandViews => "per 1K views",
        _ => type.ToString()
    };

    private static string FormatDuration(double hours)
    {
        if (hours >= 24)
            return $"{hours / 24:F1} days";
        return $"{hours:F1} hours";
    }

    private static string FormatNumber(int number)
    {
        if (number >= 1_000_000)
            return $"{number / 1_000_000.0:F1}M";
        if (number >= 1_000)
            return $"{number / 1_000.0:F1}K";
        return number.ToString();
    }
}
