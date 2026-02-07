using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services.CampaignServices;

public class ChannelApplicationService(AdMarketDbContext dbContext) : IChannelApplicationService
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
        var channel = await dbContext.Channels.FindAsync(channelId);
        if (channel is null)
            return Error.NotFound("Channel.NotFound", "Channel not found");

        if (channel.Status != ChannelStatusType.Ready)
            return Error.Validation("Channel.NotReady", "Channel is not ready to accept applications");

        if (channel.OwnerId == advertiserId)
            return Error.Validation("ChannelApplication.SelfApply", "You cannot apply to your own channel");

        var existingApplication = await dbContext.ChannelApplications
            .FirstOrDefaultAsync(a => a.ChannelId == channelId && a.AdvertiserId == advertiserId);

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

        if (application.Status != ApplicationStatusType.Pending)
            return Error.Validation("ChannelApplication.NotPending", "Application is not pending");

        application.Accept();
        await dbContext.SaveChangesAsync();

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

        if (application.Status != ApplicationStatusType.Pending)
            return Error.Validation("ChannelApplication.NotPending", "Application is not pending");

        application.Reject(reason);
        await dbContext.SaveChangesAsync();

        return application;
    }

    public async Task<ErrorOr<ChannelApplication>> WithdrawAsync(Guid id, Guid advertiserId)
    {
        var application = await dbContext.ChannelApplications
            .AsTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("ChannelApplication.NotFound", "Channel application not found");

        if (application.AdvertiserId != advertiserId)
            return Error.Forbidden("ChannelApplication.Forbidden", "You don't have permission to withdraw this application");

        if (application.Status != ApplicationStatusType.Pending)
            return Error.Validation("ChannelApplication.NotPending", "Application is not pending");

        application.Withdraw();
        await dbContext.SaveChangesAsync();

        return application;
    }
}
