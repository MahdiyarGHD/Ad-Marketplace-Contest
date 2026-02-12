using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services.CampaignServices;

public class ChannelApplicationService(
    AdMarketDbContext dbContext,
    INotificationService notificationService,
    IDealService dealService) : IChannelApplicationService
{
    public async Task<ErrorOr<ChannelApplication>> CreateAsync(
        Guid channelId,
        Guid advertiserId,
        AdFormatType proposedAdFormat,
        PriceType proposedPriceType,
        decimal proposedPriceTon,
        DateTimeOffset? proposedPostingTime = null,
        string? message = null)
    {
        var channel = await dbContext.Channels
            .Include(c => c.Pricings)
            .FirstOrDefaultAsync(c => c.Id == channelId);
        if (channel is null)
            return Error.NotFound("Channel.NotFound", "Channel not found");

        if (channel.Status != ChannelStatusType.Ready)
            return Error.Validation("Channel.NotReady", "Channel is not ready to accept applications");

        if (channel.OwnerId == advertiserId)
            return Error.Validation("ChannelApplication.SelfApply", "You cannot apply to your own channel");

        if (proposedPriceTon <= 0)
            return Error.Validation("ChannelApplication.InvalidPrice", "Proposed price must be greater than zero");

        if (proposedPostingTime.HasValue && proposedPostingTime.Value <= DateTimeOffset.UtcNow)
            return Error.Validation("ChannelApplication.InvalidPostingTime", "Proposed posting time must be in the future");

        var hasPricing = channel.Pricings.Any(p => 
            p.AdFormat == proposedAdFormat && 
            p.PriceType == proposedPriceType);

        if (!hasPricing)
            return Error.Validation("ChannelApplication.UnsupportedPricing", 
                $"Channel does not support {proposedAdFormat} with {proposedPriceType} pricing");

        var existingApplication = await dbContext.ChannelApplications
            .FirstOrDefaultAsync(a => a.ChannelId == channelId && a.AdvertiserId == advertiserId &&
                                      a.Status != ApplicationStatusType.Rejected &&
                                      a.Status != ApplicationStatusType.Withdrawn &&
                                      a.Status != ApplicationStatusType.Accepted);

        if (existingApplication is not null)
            return Error.Conflict("ChannelApplication.AlreadyExists", "You have already sent an application to this channel");

        var application = ChannelApplication.Create(
            channelId: channelId,
            advertiserId: advertiserId,
            proposedAdFormat: proposedAdFormat,
            proposedPriceType: proposedPriceType,
            proposedPriceTon: proposedPriceTon,
            proposedPostingTime: proposedPostingTime,
            message: message);

        dbContext.ChannelApplications.Add(application);
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyChannelApplicationReceivedAsync(application.Id);

        return application;
    }

    public async Task<ErrorOr<ChannelApplication>> GetByIdAsync(Guid id)
    {
        var application = await dbContext.ChannelApplications
            .Include(a => a.Channel)
            .Include(a => a.Advertiser)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("ChannelApplication.NotFound", "Channel application not found");

        return application;
    }

    public async Task<ErrorOr<List<ChannelApplication>>> GetByChannelIdAsync(Guid channelId, int skip, int take)
    {
        var applications = await dbContext.ChannelApplications
            .Include(a => a.Channel)
            .Include(a => a.Advertiser)
            .Where(a => a.ChannelId == channelId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return applications;
    }

    public async Task<ErrorOr<List<ChannelApplication>>> GetByAdvertiserIdAsync(Guid advertiserId, int skip, int take)
    {
        var applications = await dbContext.ChannelApplications
            .Include(a => a.Channel)
            .Include(a => a.Advertiser)
            .Where(a => a.AdvertiserId == advertiserId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return applications;
    }

    public async Task<ErrorOr<List<ChannelApplication>>> GetByStatusAsync(
        Guid channelId,
        ApplicationStatusType status,
        int skip,
        int take)
    {
        var applications = await dbContext.ChannelApplications
            .Include(a => a.Channel)
            .Include(a => a.Advertiser)
            .Where(a => a.ChannelId == channelId && a.Status == status)
            .OrderByDescending(a => a.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return applications;
    }

    public async Task<ErrorOr<ChannelApplication>> AcceptAsync(Guid id, Guid channelOwnerId)
    {
        var application = await dbContext.ChannelApplications
            .AsTracking()
            .Include(a => a.Channel)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("ChannelApplication.NotFound", "Channel application not found");

        if (application.Channel.OwnerId != channelOwnerId)
            return Error.Forbidden("ChannelApplication.Forbidden", "You don't have permission to accept this application");

        if (application.Status != ApplicationStatusType.Pending && application.Status != ApplicationStatusType.CounterOffer)
            return Error.Validation("ChannelApplication.InvalidStatus", "Application cannot be accepted in its current status");

        if (application.Status == ApplicationStatusType.CounterOffer && application.LastCounterByUserId == channelOwnerId)
            return Error.Validation("ChannelApplication.CannotAcceptOwn", "You cannot accept your own counter-offer");

        if (application.Status == ApplicationStatusType.CounterOffer)
            application.AcceptCounterOffer();
        else
            application.Accept();

        await dbContext.SaveChangesAsync();

        var dealResult = await dealService.CreateAsync(
            campaignId: null,
            applicationId: null,
            invitationId: null,
            channelApplicationId: application.Id,
            channelId: application.ChannelId,
            advertiserId: application.AdvertiserId,
            amountTon: application.ProposedPriceTon,
            adFormat: application.ProposedAdFormat,
            priceType: application.ProposedPriceType,
            scheduledPostTime: application.ProposedPostingTime);

        await notificationService.NotifyChannelApplicationAcceptedAsync(application.Id);

        return application;
    }

    public async Task<ErrorOr<ChannelApplication>> RejectAsync(Guid id, Guid channelOwnerId, string? reason = null)
    {
        var application = await dbContext.ChannelApplications
            .AsTracking()
            .Include(a => a.Channel)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("ChannelApplication.NotFound", "Channel application not found");

        if (application.Channel.OwnerId != channelOwnerId)
            return Error.Forbidden("ChannelApplication.Forbidden", "You don't have permission to reject this application");

        if (application.Status != ApplicationStatusType.Pending && application.Status != ApplicationStatusType.CounterOffer)
            return Error.Validation("ChannelApplication.InvalidStatus", "Application cannot be rejected in its current status");

        application.Reject(reason);
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyChannelApplicationRejectedAsync(application.Id, reason);

        return application;
    }

    public async Task<ErrorOr<ChannelApplication>> WithdrawAsync(Guid id, Guid advertiserId)
    {
        var application = await dbContext.ChannelApplications
            .AsTracking()
            .Include(a => a.Channel)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("ChannelApplication.NotFound", "Channel application not found");

        if (application.AdvertiserId != advertiserId)
            return Error.Forbidden("ChannelApplication.Forbidden", "You don't have permission to withdraw this application");

        if (application.Status != ApplicationStatusType.Pending && application.Status != ApplicationStatusType.CounterOffer)
            return Error.Validation("ChannelApplication.InvalidStatus", "Application cannot be withdrawn in its current status");

        application.Withdraw();
        await dbContext.SaveChangesAsync();

        return application;
    }

    public async Task<ErrorOr<ChannelApplication>> CounterOfferAsync(
        Guid id,
        Guid userId,
        AdFormatType adFormat,
        PriceType priceType,
        decimal priceTon,
        DateTimeOffset? postingTime = null,
        string? message = null)
    {
        var application = await dbContext.ChannelApplications
            .AsTracking()
            .Include(a => a.Channel)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("ChannelApplication.NotFound", "Channel application not found");

        var isOwner = application.Channel.OwnerId == userId;
        var isAdvertiser = application.AdvertiserId == userId;

        if (!isOwner && !isAdvertiser)
            return Error.Forbidden("ChannelApplication.Forbidden", "You don't have permission to counter this application");

        if (application.Status != ApplicationStatusType.Pending && application.Status != ApplicationStatusType.CounterOffer)
            return Error.Validation("ChannelApplication.InvalidStatus", "Application cannot receive counter-offers in its current status");

        if (application.Status == ApplicationStatusType.CounterOffer && application.LastCounterByUserId == userId)
            return Error.Validation("ChannelApplication.ConsecutiveCounter", "You cannot send consecutive counter-offers");

        if (priceTon <= 0)
            return Error.Validation("ChannelApplication.InvalidPrice", "Counter-offer price must be greater than zero");

        if (postingTime.HasValue && postingTime.Value <= DateTimeOffset.UtcNow)
            return Error.Validation("ChannelApplication.InvalidPostingTime", "Posting time must be in the future");

        application.CounterOffer(userId, adFormat, priceType, priceTon, postingTime, message);
        await dbContext.SaveChangesAsync();

        await notificationService.NotifyChannelApplicationCounterOfferAsync(application.Id);

        return application;
    }

    public async Task<ErrorOr<ChannelApplication>> AcceptCounterOfferAsync(Guid id, Guid userId)
    {
        var application = await dbContext.ChannelApplications
            .AsTracking()
            .Include(a => a.Channel)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("ChannelApplication.NotFound", "Channel application not found");

        var isOwner = application.Channel.OwnerId == userId;
        var isAdvertiser = application.AdvertiserId == userId;

        if (!isOwner && !isAdvertiser)
            return Error.Forbidden("ChannelApplication.Forbidden", "You don't have permission");

        if (application.Status != ApplicationStatusType.CounterOffer)
            return Error.Validation("ChannelApplication.NoCounterOffer", "No counter-offer to accept");

        if (application.LastCounterByUserId == userId)
            return Error.Validation("ChannelApplication.CannotAcceptOwn", "You cannot accept your own counter-offer");

        application.AcceptCounterOffer();
        await dbContext.SaveChangesAsync();

        var dealResult = await dealService.CreateAsync(
            campaignId: null,
            applicationId: null,
            invitationId: null,
            channelApplicationId: application.Id,
            channelId: application.ChannelId,
            advertiserId: application.AdvertiserId,
            amountTon: application.ProposedPriceTon,
            adFormat: application.ProposedAdFormat,
            priceType: application.ProposedPriceType,
            scheduledPostTime: application.ProposedPostingTime);

        await notificationService.NotifyChannelApplicationAcceptedAsync(application.Id);

        return application;
    }
}
