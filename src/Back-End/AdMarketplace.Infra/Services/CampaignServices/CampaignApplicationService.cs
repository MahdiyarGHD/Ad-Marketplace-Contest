using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services.CampaignServices;

public class CampaignApplicationService(AdMarketDbContext dbContext) : ICampaignApplicationService
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

        var channel = await dbContext.Channels.FindAsync(channelId);
        if (channel is null)
            return Error.NotFound("Channel.NotFound", "Channel not found");

        var existingApplication = await dbContext.CampaignApplications
            .FirstOrDefaultAsync(a => a.CampaignId == campaignId && a.ChannelId == channelId);

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
            return Error.NotFound("Application.NotFound", "Application not found");

        if (application.Campaign.AdvertiserId != advertiserId)
            return Error.Forbidden("Application.Forbidden", "You don't have permission to accept this application");

        if (application.Status != ApplicationStatusType.Pending)
            return Error.Validation("Application.NotPending", "Application is not pending");

        application.Accept();
        await dbContext.SaveChangesAsync();

        return application;
    }

    public async Task<ErrorOr<CampaignApplication>> RejectAsync(Guid id, Guid advertiserId, string? reason = null)
    {
        var application = await dbContext.CampaignApplications
            .AsTracking()
            .Include(a => a.Campaign)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("Application.NotFound", "Application not found");

        if (application.Campaign.AdvertiserId != advertiserId)
            return Error.Forbidden("Application.Forbidden", "You don't have permission to reject this application");

        if (application.Status != ApplicationStatusType.Pending)
            return Error.Validation("Application.NotPending", "Application is not pending");

        application.Reject(reason);
        await dbContext.SaveChangesAsync();

        return application;
    }

    public async Task<ErrorOr<CampaignApplication>> WithdrawAsync(Guid id, Guid channelOwnerId)
    {
        var application = await dbContext.CampaignApplications
            .AsTracking()
            .Include(a => a.Channel)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application is null)
            return Error.NotFound("Application.NotFound", "Application not found");

        if (application.Channel.OwnerId != channelOwnerId)
            return Error.Forbidden("Application.Forbidden", "You don't have permission to withdraw this application");

        if (application.Status != ApplicationStatusType.Pending)
            return Error.Validation("Application.NotPending", "Application is not pending");

        application.Withdraw();
        await dbContext.SaveChangesAsync();

        return application;
    }
}
