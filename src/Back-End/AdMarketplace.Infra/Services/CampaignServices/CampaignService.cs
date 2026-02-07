using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services.CampaignServices;

public class CampaignService(AdMarketDbContext dbContext) : ICampaignService
{
    public async Task<ErrorOr<Campaign>> CreateAsync(
        Guid advertiserId,
        string title,
        string description,
        decimal budgetTon,
        Guid? categoryId = null,
        string? brief = null,
        decimal? maxPricePerPlacement = null,
        CampaignTargetingContract? targeting = null,
        CampaignCreativeContract? creative = null,
        DateTimeOffset? startsAt = null,
        DateTimeOffset? endsAt = null,
        DateTimeOffset? applicationDeadline = null)
    {
        var advertiser = await dbContext.Users.FindAsync(advertiserId);
        if (advertiser is null)
            return Error.NotFound("User.NotFound", "Advertiser not found");

        if (categoryId.HasValue)
        {
            var categoryExists = await dbContext.Categories.AnyAsync(c => c.Id == categoryId.Value);
            if (!categoryExists)
                return Error.NotFound("Category.NotFound", "Category not found");
        }

        var campaign = Campaign.Create(
            advertiserId: advertiserId,
            title: title,
            description: description,
            budgetTon: budgetTon,
            categoryId: categoryId,
            brief: brief,
            maxPricePerPlacement: maxPricePerPlacement,
            targeting: targeting,
            creative: creative,
            startsAt: startsAt,
            endsAt: endsAt,
            applicationDeadline: applicationDeadline);

        dbContext.Campaigns.Add(campaign);
        await dbContext.SaveChangesAsync();

        return campaign;
    }

    public async Task<ErrorOr<Campaign>> GetByIdAsync(Guid id)
    {
        var campaign = await dbContext.Campaigns
            .Include(c => c.Advertiser)
            .Include(c => c.Category)
            .Include(c => c.Applications)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (campaign is null)
            return Error.NotFound("Campaign.NotFound", "Campaign not found");

        return campaign;
    }

    public async Task<ErrorOr<List<Campaign>>> GetByAdvertiserIdAsync(Guid advertiserId, int skip, int take)
    {
        var campaigns = await dbContext.Campaigns
            .Include(c => c.Advertiser)
            .Include(c => c.Category)
            .Where(c => c.AdvertiserId == advertiserId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return campaigns;
    }

    public async Task<ErrorOr<List<Campaign>>> GetActiveAsync(int skip, int take)
    {
        var campaigns = await dbContext.Campaigns
            .Include(c => c.Advertiser)
            .Include(c => c.Category)
            .Where(c => c.Status == CampaignStatusType.Active)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return campaigns;
    }

    public async Task<ErrorOr<List<Campaign>>> GetByCategoryIdAsync(Guid categoryId, int skip, int take)
    {
        var campaigns = await dbContext.Campaigns
            .Include(c => c.Advertiser)
            .Include(c => c.Category)
            .Where(c => c.CategoryId == categoryId && c.Status == CampaignStatusType.Active)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return campaigns;
    }

    public async Task<ErrorOr<List<Campaign>>> SearchAsync(
        Guid? categoryId = null,
        decimal? minBudget = null,
        decimal? maxBudget = null,
        CampaignStatusType? status = null,
        AdFormatType? adFormat = null,
        PriceType? priceType = null,
        int skip = 0,
        int take = 20)
    {
        var query = dbContext.Campaigns
            .Include(c => c.Advertiser)
            .Include(c => c.Category)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(c => c.CategoryId == categoryId.Value);

        if (minBudget.HasValue)
            query = query.Where(c => c.BudgetTon >= minBudget.Value);

        if (maxBudget.HasValue)
            query = query.Where(c => c.BudgetTon <= maxBudget.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        var campaigns = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        if (adFormat.HasValue)
            campaigns = campaigns
                .Where(c => c.TargetingJson?.PreferredAdFormats?.Contains(adFormat.Value) == true)
                .ToList();

        if (priceType.HasValue)
            campaigns = campaigns
                .Where(c => c.TargetingJson?.PreferredPriceTypes?.Contains(priceType.Value) == true)
                .ToList();

        return campaigns
            .Skip(skip)
            .Take(take)
            .ToList();
    }

    public async Task<ErrorOr<Campaign>> UpdateAsync(
        Guid id,
        Guid advertiserId,
        string? title = null,
        string? description = null,
        string? brief = null,
        decimal? budgetTon = null,
        decimal? maxPricePerPlacement = null,
        Guid? categoryId = null,
        CampaignTargetingContract? targeting = null,
        CampaignCreativeContract? creative = null,
        DateTimeOffset? startsAt = null,
        DateTimeOffset? endsAt = null,
        DateTimeOffset? applicationDeadline = null)
    {
        var campaign = await dbContext.Campaigns
            .AsTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
        if (campaign is null)
            return Error.NotFound("Campaign.NotFound", "Campaign not found");

        if (campaign.AdvertiserId != advertiserId)
            return Error.Forbidden("Campaign.Forbidden", "You don't have permission to update this campaign");

        campaign.Update(
            title: title,
            description: description,
            brief: brief,
            budgetTon: budgetTon,
            maxPricePerPlacement: maxPricePerPlacement,
            categoryId: categoryId,
            targeting: targeting,
            creative: creative,
            startsAt: startsAt,
            endsAt: endsAt,
            applicationDeadline: applicationDeadline);

        await dbContext.SaveChangesAsync();

        return campaign;
    }

    public async Task<ErrorOr<Campaign>> UpdateStatusAsync(Guid id, Guid advertiserId, CampaignStatusType status)
    {
        var campaign = await dbContext.Campaigns
            .AsTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
        if (campaign is null)
            return Error.NotFound("Campaign.NotFound", "Campaign not found");

        if (campaign.AdvertiserId != advertiserId)
            return Error.Forbidden("Campaign.Forbidden", "You don't have permission to update this campaign");

        campaign.UpdateStatus(status);
        await dbContext.SaveChangesAsync();

        return campaign;
    }

    public async Task<ErrorOr<bool>> DeleteAsync(Guid id, Guid advertiserId)
    {
        var campaign = await dbContext.Campaigns
            .AsTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
        if (campaign is null)
            return Error.NotFound("Campaign.NotFound", "Campaign not found");

        if (campaign.AdvertiserId != advertiserId)
            return Error.Forbidden("Campaign.Forbidden", "You don't have permission to delete this campaign");

        dbContext.Campaigns.Remove(campaign);
        await dbContext.SaveChangesAsync();

        return true;
    }
}
