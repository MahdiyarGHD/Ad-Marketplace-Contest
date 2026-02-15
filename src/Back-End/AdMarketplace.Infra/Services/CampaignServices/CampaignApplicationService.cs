using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services.CampaignServices;

public class CampaignApplicationService(
    AdMarketDbContext dbContext,
    INotificationService notificationService,
    IDealService dealService) : ICampaignApplicationService
{
    public async Task<ErrorOr<CampaignApplication>> CreateAsync(
        Guid campaignId,
        Guid channelId,
        AdFormatType proposedAdFormat,
        PriceType proposedPriceType,
        decimal proposedPriceTon,
        DateTimeOffset? proposedPostingTime = null,
        string? message = null)
    {
        var campaign = await dbContext.Campaigns.FindAsync(campaignId);
        if (campaign is null)
            return Error.NotFound("Campaign.NotFound", "Campaign not found");

        if (campaign.Status != CampaignStatusType.Active)
            return Error.Validation("Campaign.NotActive", "Campaign is not active");

        if (campaign.ApplicationDeadline.HasValue && campaign.ApplicationDeadline.Value <= DateTimeOffset.UtcNow)
            return Error.Validation("Campaign.DeadlinePassed", "The application deadline for this campaign has passed");

        var channel = await dbContext.Channels
            .Include(c => c.Pricings)
            .FirstOrDefaultAsync(c => c.Id == channelId);
        if (channel is null)
            return Error.NotFound("Channel.NotFound", "Channel not found");

        if (channel.OwnerId == campaign.AdvertiserId)
            return Error.Validation("Application.SelfDeal", "You cannot apply your own channel to your own campaign");

        var hasPricing = channel.Pricings.Any(p => 
            p.AdFormat == proposedAdFormat && 
            p.PriceType == proposedPriceType);

        if (!hasPricing)
            return Error.Validation("Application.UnsupportedPricing", 
                $"Channel does not support {proposedAdFormat} with {proposedPriceType} pricing");

        if (proposedPriceTon <= 0)
            return Error.Validation("Application.InvalidPrice", "Proposed price must be greater than zero");

        if (campaign.MaxPricePerPlacement.HasValue && proposedPriceTon > campaign.MaxPricePerPlacement.Value)
            return Error.Validation("Application.PriceExceedsMax", "Proposed price exceeds the campaign's maximum price per placement");

        if (proposedPostingTime.HasValue && proposedPostingTime.Value <= DateTimeOffset.UtcNow)
            return Error.Validation("Application.InvalidPostingTime", "Proposed posting time must be in the future");

        var existingApplication = await dbContext.CampaignApplications
            .FirstOrDefaultAsync(a => a.CampaignId == campaignId && a.ChannelId == channelId &&
                                      a.Status != ApplicationStatusType.Rejected &&
                                      a.Status != ApplicationStatusType.Withdrawn &&
                                      a.Status != ApplicationStatusType.Accepted);

        if (existingApplication is not null)
            return Error.Conflict("Application.AlreadyExists", "You have already applied to this campaign");

        var application = CampaignApplication.Create(
            campaignId: campaignId,
            channelId: channelId,
            proposedAdFormat: proposedAdFormat,
            proposedPriceType: proposedPriceType,
            proposedPriceTon: proposedPriceTon,
            proposedPostingTime: proposedPostingTime,
            message: message);

        dbContext.CampaignApplications.Add(application);
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyCampaignApplicationReceivedAsync(application.Id);

        return application;
    }

    public async Task<ErrorOr<CampaignApplication>> GetByIdAsync(Guid id)
    {
        var application = await dbContext.CampaignApplications
            .Include(a => a.Campaign)
            .Include(a => a.Channel)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("Application.NotFound", "Application not found");

        return application;
    }

    public async Task<ErrorOr<List<CampaignApplication>>> GetByCampaignIdAsync(Guid campaignId, int skip, int take)
    {
        var applications = await dbContext.CampaignApplications
            .Include(a => a.Campaign)
            .Include(a => a.Channel)
                .ThenInclude(c => c.Pricings)
            .Include(a => a.Channel)
                .ThenInclude(c => c.Category)
            .Where(a => a.CampaignId == campaignId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return applications;
    }

    public async Task<ErrorOr<List<CampaignApplication>>> GetByChannelIdAsync(Guid channelId, int skip, int take)
    {
        var applications = await dbContext.CampaignApplications
            .Include(a => a.Campaign)
            .Include(a => a.Channel)
            .Where(a => a.ChannelId == channelId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return applications;
    }

    public async Task<ErrorOr<List<CampaignApplication>>> GetByStatusAsync(
        Guid campaignId,
        ApplicationStatusType status,
        int skip,
        int take)
    {
        var applications = await dbContext.CampaignApplications
            .Include(a => a.Campaign)
            .Include(a => a.Channel)
            .Where(a => a.CampaignId == campaignId && a.Status == status)
            .OrderByDescending(a => a.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return applications;
    }

    public async Task<ErrorOr<CampaignApplication>> AcceptAsync(Guid id, Guid advertiserId)
    {
        var application = await dbContext.CampaignApplications
            .AsTracking()
            .Include(a => a.Campaign)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("CampaignApplication.NotFound", "Application not found");

        if (application.Campaign.AdvertiserId != advertiserId)
            return Error.Forbidden("CampaignApplication.Forbidden", "You don't have permission to accept this application");

        if (application.Status != ApplicationStatusType.Pending && application.Status != ApplicationStatusType.CounterOffer)
            return Error.Validation("CampaignApplication.InvalidStatus", "Application cannot be accepted in its current status");

        if (application.Status == ApplicationStatusType.CounterOffer && application.LastCounterByUserId == advertiserId)
            return Error.Validation("CampaignApplication.CannotAcceptOwn", "You cannot accept your own counter-offer");

        if (application.Campaign.Status != CampaignStatusType.Active)
            return Error.Validation("Campaign.NotActive", "Campaign is no longer active");

        if (application.Status == ApplicationStatusType.CounterOffer)
            application.AcceptCounterOffer();
        else
            application.Accept();

        await dbContext.SaveChangesAsync();

        var dealResult = await dealService.CreateAsync(
            campaignId: application.CampaignId,
            applicationId: application.Id,
            invitationId: null,
            channelApplicationId: null,
            channelId: application.ChannelId,
            advertiserId: application.Campaign.AdvertiserId,
            amountTon: application.ProposedPriceTon,
            adFormat: application.ProposedAdFormat,
            priceType: application.ProposedPriceType,
            scheduledPostTime: application.ProposedPostingTime);

        await notificationService.NotifyCampaignApplicationAcceptedAsync(application.Id);

        return application;
    }

    public async Task<ErrorOr<CampaignApplication>> RejectAsync(Guid id, Guid advertiserId, string? reason = null)
    {
        var application = await dbContext.CampaignApplications
            .AsTracking()
            .Include(a => a.Campaign)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("CampaignApplication.NotFound", "Application not found");

        if (application.Campaign.AdvertiserId != advertiserId)
            return Error.Forbidden("CampaignApplication.Forbidden", "You don't have permission to reject this application");

        if (application.Status != ApplicationStatusType.Pending && application.Status != ApplicationStatusType.CounterOffer)
            return Error.Validation("CampaignApplication.InvalidStatus", "Application cannot be rejected in its current status");

        application.Reject(reason);
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyCampaignApplicationRejectedAsync(application.Id, reason);

        return application;
    }

    public async Task<ErrorOr<CampaignApplication>> WithdrawAsync(Guid id, Guid channelOwnerId)
    {
        var application = await dbContext.CampaignApplications
            .AsTracking()
            .Include(a => a.Channel)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("CampaignApplication.NotFound", "Application not found");

        if (application.Channel.OwnerId != channelOwnerId)
            return Error.Forbidden("CampaignApplication.Forbidden", "You don't have permission to withdraw this application");

        if (application.Status != ApplicationStatusType.Pending && application.Status != ApplicationStatusType.CounterOffer)
            return Error.Validation("CampaignApplication.InvalidStatus", "Application cannot be withdrawn in its current status");

        application.Withdraw();
        await dbContext.SaveChangesAsync();

        return application;
    }

    public async Task<ErrorOr<CampaignApplication>> CounterOfferAsync(
        Guid id,
        Guid userId,
        AdFormatType adFormat,
        PriceType priceType,
        decimal priceTon,
        DateTimeOffset? postingTime = null,
        string? message = null)
    {
        var application = await dbContext.CampaignApplications
            .AsTracking()
            .Include(a => a.Campaign)
            .Include(a => a.Channel)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("CampaignApplication.NotFound", "Application not found");

        var isAdvertiser = application.Campaign.AdvertiserId == userId;
        var isOwner = application.Channel.OwnerId == userId;

        if (!isAdvertiser && !isOwner)
            return Error.Forbidden("CampaignApplication.Forbidden", "You don't have permission to counter this application");

        if (application.Status != ApplicationStatusType.Pending && application.Status != ApplicationStatusType.CounterOffer)
            return Error.Validation("CampaignApplication.InvalidStatus", "Application cannot receive counter-offers in its current status");

        if (application.Status == ApplicationStatusType.CounterOffer && application.LastCounterByUserId == userId)
            return Error.Validation("CampaignApplication.ConsecutiveCounter", "You cannot send consecutive counter-offers");

        if (priceTon <= 0)
            return Error.Validation("CampaignApplication.InvalidPrice", "Counter-offer price must be greater than zero");

        if (postingTime.HasValue && postingTime.Value <= DateTimeOffset.UtcNow)
            return Error.Validation("CampaignApplication.InvalidPostingTime", "Posting time must be in the future");

        application.CounterOffer(userId, adFormat, priceType, priceTon, postingTime, message);
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyCampaignApplicationCounterOfferAsync(application.Id);

        return application;
    }

    public async Task<ErrorOr<CampaignApplication>> AcceptCounterOfferAsync(Guid id, Guid userId)
    {
        var application = await dbContext.CampaignApplications
            .AsTracking()
            .Include(a => a.Campaign)
            .Include(a => a.Channel)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("CampaignApplication.NotFound", "Application not found");

        var isAdvertiser = application.Campaign.AdvertiserId == userId;
        var isOwner = application.Channel.OwnerId == userId;

        if (!isAdvertiser && !isOwner)
            return Error.Forbidden("CampaignApplication.Forbidden", "You don't have permission");

        if (application.Status != ApplicationStatusType.CounterOffer)
            return Error.Validation("CampaignApplication.NoCounterOffer", "No counter-offer to accept");

        if (application.LastCounterByUserId == userId)
            return Error.Validation("CampaignApplication.CannotAcceptOwn", "You cannot accept your own counter-offer");

        application.AcceptCounterOffer();
        await dbContext.SaveChangesAsync();

        var dealResult = await dealService.CreateAsync(
            campaignId: application.CampaignId,
            applicationId: application.Id,
            invitationId: null,
            channelApplicationId: null,
            channelId: application.ChannelId,
            advertiserId: application.Campaign.AdvertiserId,
            amountTon: application.ProposedPriceTon,
            adFormat: application.ProposedAdFormat,
            priceType: application.ProposedPriceType,
            scheduledPostTime: application.ProposedPostingTime);

        await notificationService.NotifyCampaignApplicationAcceptedAsync(application.Id);

        return application;
    }
}
