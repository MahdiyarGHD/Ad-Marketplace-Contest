using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services.CampaignServices;

public class DealService(
    AdMarketDbContext dbContext,
    INotificationService notificationService) : IDealService
{
    public async Task<ErrorOr<Deal>> CreateAsync(
        Guid? campaignId,
        Guid? applicationId,
        Guid? invitationId,
        Guid? channelApplicationId,
        Guid channelId,
        Guid advertiserId,
        decimal amountTon,
        AdFormatType adFormat,
        PriceType priceType,
        DateTimeOffset? scheduledPostTime = null,
        string? escrowWalletAddress = null)
    {
        var sourceCount = new[] { applicationId.HasValue, invitationId.HasValue, channelApplicationId.HasValue }.Count(x => x);

        if (sourceCount == 0)
            return Error.Validation("Deal.InvalidSource", "Deal must be created from an application, invitation, or channel application");

        if (sourceCount > 1)
            return Error.Validation("Deal.InvalidSource", "Deal can only be created from a single source");

        if (applicationId.HasValue)
        {
            var application = await dbContext.CampaignApplications
                .Include(a => a.Campaign)
                .Include(a => a.Deal)
                .FirstOrDefaultAsync(a => a.Id == applicationId.Value);

            if (application is null)
                return Error.NotFound("Application.NotFound", "Application not found");

            if (application.Status != ApplicationStatusType.Accepted)
                return Error.Validation("Application.NotAccepted", "Application must be accepted first");

            if (application.Deal is not null)
                return Error.Conflict("Deal.AlreadyExists", "Deal already exists for this application");

            if (application.Campaign.AdvertiserId != advertiserId)
                return Error.Forbidden("Deal.Forbidden", "You don't have permission to create a deal for this application");

            if (application.ChannelId != channelId)
                return Error.Validation("Deal.ChannelMismatch", "Channel ID does not match the application's channel");

            if (application.CampaignId != campaignId)
                return Error.Validation("Deal.CampaignMismatch", "Campaign ID does not match the application's campaign");

            if (application.Campaign.Status != CampaignStatusType.Active)
                return Error.Validation("Campaign.NotActive", "Campaign is no longer active");
        }

        if (invitationId.HasValue)
        {
            var invitation = await dbContext.CampaignInvitations
                .Include(i => i.Campaign)
                .Include(i => i.Deal)
                .FirstOrDefaultAsync(i => i.Id == invitationId.Value);

            if (invitation is null)
                return Error.NotFound("Invitation.NotFound", "Invitation not found");

            if (invitation.Status != InvitationStatusType.Accepted)
                return Error.Validation("Invitation.NotAccepted", "Invitation must be accepted first");

            if (invitation.Deal is not null)
                return Error.Conflict("Deal.AlreadyExists", "Deal already exists for this invitation");

            if (invitation.Campaign.AdvertiserId != advertiserId)
                return Error.Forbidden("Deal.Forbidden", "You don't have permission to create a deal for this invitation");

            if (invitation.ChannelId != channelId)
                return Error.Validation("Deal.ChannelMismatch", "Channel ID does not match the invitation's channel");

            if (campaignId.HasValue && invitation.CampaignId != campaignId.Value)
                return Error.Validation("Deal.CampaignMismatch", "Campaign ID does not match the invitation's campaign");

            if (invitation.Campaign.Status != CampaignStatusType.Active)
                return Error.Validation("Campaign.NotActive", "Campaign is no longer active");
        }

        if (channelApplicationId.HasValue)
        {
            var channelApplication = await dbContext.ChannelApplications
                .Include(a => a.Channel)
                .Include(a => a.Deal)
                .FirstOrDefaultAsync(a => a.Id == channelApplicationId.Value);

            if (channelApplication is null)
                return Error.NotFound("ChannelApplication.NotFound", "Channel application not found");

            if (channelApplication.Status != ApplicationStatusType.Accepted)
                return Error.Validation("ChannelApplication.NotAccepted", "Channel application must be accepted first");

            if (channelApplication.Deal is not null)
                return Error.Conflict("Deal.AlreadyExists", "Deal already exists for this channel application");

            if (channelApplication.AdvertiserId != advertiserId)
                return Error.Forbidden("Deal.Forbidden", "You don't have permission to create a deal for this channel application");

            if (channelApplication.ChannelId != channelId)
                return Error.Validation("Deal.ChannelMismatch", "Channel ID does not match the channel application's channel");
        }

        var channelPricing = await dbContext.ChannelPricings
            .FirstOrDefaultAsync(p => p.ChannelId == channelId && p.AdFormat == adFormat && p.PriceType == priceType);

        var channelUnitPrice = channelPricing?.PriceTon ?? 0m;

        var deal = Deal.Create(
            campaignId: campaignId,
            applicationId: applicationId,
            invitationId: invitationId,
            channelApplicationId: channelApplicationId,
            channelId: channelId,
            advertiserId: advertiserId,
            amountTon: amountTon,
            adFormat: adFormat,
            priceType: priceType,
            channelUnitPrice: channelUnitPrice,
            scheduledPostTime: scheduledPostTime,
            escrowWalletAddress: escrowWalletAddress);

        dbContext.Deals.Add(deal);
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyDealCreatedAsync(deal.Id);

        return deal;
    }

    public async Task<ErrorOr<Deal>> GetByIdAsync(Guid id)
    {
        var deal = await dbContext.Deals
            .Include(d => d.Campaign)
            .Include(d => d.Application)
            .Include(d => d.ChannelApplication)
            .Include(d => d.Channel)
            .Include(d => d.Advertiser)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (deal is null)
            return Error.NotFound("Deal.NotFound", "Deal not found");

        return deal;
    }

    public async Task<ErrorOr<Deal>> GetByApplicationIdAsync(Guid applicationId)
    {
        var deal = await dbContext.Deals
            .Include(d => d.Campaign)
            .Include(d => d.Application)
            .Include(d => d.ChannelApplication)
            .Include(d => d.Channel)
            .Include(d => d.Advertiser)
            .FirstOrDefaultAsync(d => d.ApplicationId == applicationId);

        if (deal is null)
            return Error.NotFound("Deal.NotFound", "Deal not found");

        return deal;
    }

    public async Task<ErrorOr<List<Deal>>> GetByUserIdAsync(Guid userId, int skip, int take)
    {
        var deals = await dbContext.Deals
            .Include(d => d.Campaign)
            .Include(d => d.Channel)
            .Include(d => d.Advertiser)
            .Where(d => d.AdvertiserId == userId || d.Channel.OwnerId == userId)
            .OrderByDescending(d => d.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return deals;
    }

    public async Task<ErrorOr<List<Deal>>> GetByAdvertiserIdAsync(Guid advertiserId, int skip, int take)
    {
        var deals = await dbContext.Deals
            .Include(d => d.Campaign)
            .Include(d => d.Channel)
            .Where(d => d.AdvertiserId == advertiserId)
            .OrderByDescending(d => d.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return deals;
    }

    public async Task<ErrorOr<List<Deal>>> GetByChannelIdAsync(Guid channelId, int skip, int take)
    {
        var deals = await dbContext.Deals
            .Include(d => d.Campaign)
            .Include(d => d.Channel)
            .Include(d => d.Advertiser)
            .Where(d => d.ChannelId == channelId)
            .OrderByDescending(d => d.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return deals;
    }

    public async Task<ErrorOr<List<Deal>>> GetByCampaignIdAsync(Guid campaignId, int skip, int take)
    {
        var deals = await dbContext.Deals
            .Include(d => d.Campaign)
            .Include(d => d.Channel)
            .Where(d => d.CampaignId == campaignId)
            .OrderByDescending(d => d.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return deals;
    }

    public async Task<ErrorOr<List<Deal>>> GetByStatusAsync(DealStatusType status, int skip, int take)
    {
        var deals = await dbContext.Deals
            .Include(d => d.Campaign)
            .Include(d => d.Channel)
            .Include(d => d.Advertiser)
            .Where(d => d.Status == status)
            .OrderByDescending(d => d.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return deals;
    }

    public async Task<ErrorOr<List<Deal>>> GetExpiredDealsAsync()
    {
        var now = DateTimeOffset.UtcNow;
        var deals = await dbContext.Deals
            .Include(d => d.Campaign)
            .Include(d => d.Channel)
            .Where(d => d.AutoCancelAt.HasValue && d.AutoCancelAt.Value <= now && 
                       d.Status != DealStatusType.Completed && 
                       d.Status != DealStatusType.Cancelled &&
                       d.Status != DealStatusType.Refunded)
            .ToListAsync();

        return deals;
    }

    public async Task<ErrorOr<Deal>> FundEscrowAsync(Guid id, Guid advertiserId, string transactionHash, string walletAddress)
    {
        var deal = await dbContext.Deals
            .AsTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        if (deal is null)
            return Error.NotFound("Deal.NotFound", "Deal not found");

        if (deal.AdvertiserId != advertiserId)
            return Error.Forbidden("Deal.Forbidden", "You don't have permission to fund this deal");

        if (deal.Status != DealStatusType.AwaitingPayment)
            return Error.Validation("Deal.InvalidStatus", "Deal is not awaiting payment");

        if (deal.AutoCancelAt.HasValue && deal.AutoCancelAt.Value <= DateTimeOffset.UtcNow)
            return Error.Validation("Deal.Expired", "Deal has expired and can no longer be funded");

        // var advertiser = await dbContext.Users
        //     .AsTracking()
        //     .FirstOrDefaultAsync(user => user.Id.Equals(deal.AdvertiserId) && user.Balance >= deal.AmountTon);
        //
        // if (advertiser is null)
        //     return Error.Validation("Deal.NotEnoughCredit", "Advertiser doesn't have enough credit");
        //
        // advertiser.Deduct(deal.AmountTon);
        
        deal.FundEscrow(transactionHash, walletAddress);
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyEscrowFundedAsync(deal.Id);

        return deal;
    }

    public async Task<ErrorOr<Deal>> SubmitDraftAsync(Guid id, Guid channelOwnerId, long messageId)
    {
        var deal = await dbContext.Deals
            .AsTracking()
            .Include(d => d.Channel)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (deal is null)
            return Error.NotFound("Deal.NotFound", "Deal not found");

        if (deal.Channel.OwnerId != channelOwnerId)
            return Error.Forbidden("Deal.Forbidden", "You don't have permission to submit draft for this deal");

        if (deal.Status != DealStatusType.EscrowFunded && deal.Status != DealStatusType.DraftRejected)
            return Error.Validation("Deal.InvalidStatus", "Cannot submit draft in current status");

        deal.SubmitDraft(messageId);
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyDraftSubmittedAsync(deal.Id);

        return deal;
    }

    public async Task<ErrorOr<Deal>> ApproveDraftAsync(Guid id, Guid advertiserId)
    {
        var deal = await dbContext.Deals
            .AsTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        if (deal is null)
            return Error.NotFound("Deal.NotFound", "Deal not found");

        if (deal.AdvertiserId != advertiserId)
            return Error.Forbidden("Deal.Forbidden", "You don't have permission to approve draft for this deal");

        if (deal.Status != DealStatusType.DraftSubmitted)
            return Error.Validation("Deal.InvalidStatus", "No draft to approve");

        deal.ApproveDraft();
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyDraftApprovedAsync(deal.Id);

        return deal;
    }

    public async Task<ErrorOr<Deal>> RejectDraftAsync(Guid id, Guid advertiserId, string feedback)
    {
        var deal = await dbContext.Deals
            .AsTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        if (deal is null)
            return Error.NotFound("Deal.NotFound", "Deal not found");

        if (deal.AdvertiserId != advertiserId)
            return Error.Forbidden("Deal.Forbidden", "You don't have permission to reject draft for this deal");

        if (deal.Status != DealStatusType.DraftSubmitted)
            return Error.Validation("Deal.InvalidStatus", "No draft to reject");

        deal.RejectDraft(feedback);
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyDraftRejectedAsync(deal.Id, feedback);

        return deal;
    }

    public async Task<ErrorOr<Deal>> MarkAsPostedAsync(Guid id, long messageId)
    {
        var deal = await dbContext.Deals
            .AsTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        if (deal is null)
            return Error.NotFound("Deal.NotFound", "Deal not found");

        if (deal.Status != DealStatusType.DraftApproved && deal.Status != DealStatusType.Scheduled)
            return Error.Validation("Deal.InvalidStatus", "Deal is not ready to be posted");

        deal.MarkAsPosted(messageId);
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyDealPostedAsync(deal.Id);

        return deal;
    }

    public async Task<ErrorOr<Deal>> ReleaseFundsAsync(Guid id)
    {
        var deal = await dbContext.Deals
            .AsTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        if (deal is null)
            return Error.NotFound("Deal.NotFound", "Deal not found");

        if (deal.Status != DealStatusType.Verifying)
            return Error.Validation("Deal.InvalidStatus", "Deal is not ready for fund release");

        deal.ReleaseFunds();
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyDealCompletedAsync(deal.Id);

        return deal;
    }

    public async Task<ErrorOr<Deal>> RefundAsync(Guid id)
    {
        var deal = await dbContext.Deals
            .AsTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        if (deal is null)
            return Error.NotFound("Deal.NotFound", "Deal not found");

        if (deal.Status != DealStatusType.EscrowFunded &&
            deal.Status != DealStatusType.DraftSubmitted &&
            deal.Status != DealStatusType.DraftRejected &&
            deal.Status != DealStatusType.DraftApproved &&
            deal.Status != DealStatusType.Scheduled &&
            deal.Status != DealStatusType.Disputed)
            return Error.Validation("Deal.InvalidStatus", "Deal cannot be refunded in its current status");

        deal.Refund();
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyDealRefundedAsync(deal.Id);

        return deal;
    }

    public async Task<ErrorOr<Deal>> UpdateStatusAsync(Guid id, DealStatusType status)
    {
        var deal = await dbContext.Deals
            .AsTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        if (deal is null)
            return Error.NotFound("Deal.NotFound", "Deal not found");

        deal.UpdateStatus(status);
        await dbContext.SaveChangesAsync();

        return deal;
    }

    public async Task<ErrorOr<Deal>> ResolveDisputeAsync(Guid id, DisputeResolutionType resolution)
    {
        var deal = await dbContext.Deals
            .AsTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        if (deal is null)
            return Error.NotFound("Deal.NotFound", "Deal not found");

        if (deal.Status != DealStatusType.Disputed)
            return Error.Validation("Deal.NotDisputed", "Deal is not in disputed status");

        switch (resolution)
        {
            case DisputeResolutionType.RefundAdvertiser:
                deal.ResolveDisputeWithRefund();
                break;
            case DisputeResolutionType.ReleaseToOwner:
                deal.ResolveDisputeWithRelease();
                break;
            default:
                return Error.Validation("Deal.InvalidResolution", "Invalid dispute resolution type");
        }

        await dbContext.SaveChangesAsync();
        await notificationService.NotifyDisputeResolvedAsync(deal.Id, resolution);

        return deal;
    }
}
