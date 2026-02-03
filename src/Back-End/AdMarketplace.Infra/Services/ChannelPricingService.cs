using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services;

public class ChannelPricingService(AdMarketDbContext dbContext) : IChannelPricingService
{
    public async Task<ErrorOr<ChannelPricing>> CreateAsync(
        Guid channelId,
        AdFormatType adFormat,
        PriceType priceType,
        decimal priceTon)
    {
        var existingPricing = await dbContext.ChannelPricings
            .FirstOrDefaultAsync(p => p.ChannelId == channelId && p.AdFormat == adFormat && p.PriceType == priceType);

        if (existingPricing is not null)
            return Error.Conflict("ChannelPricing.AlreadyExists", "Pricing for this format and type already exists");

        if (priceTon <= 0)
            return Error.Validation("ChannelPricing.InvalidPrice", "Price must be greater than zero");

        var pricing = ChannelPricing.Create(channelId, adFormat, priceType, priceTon);

        dbContext.ChannelPricings.Add(pricing);
        await dbContext.SaveChangesAsync();

        return pricing;
    }

    public async Task<ErrorOr<List<ChannelPricing>>> CreateBulkAsync(
        Guid channelId,
        List<(AdFormatType AdFormat, PriceType PriceType, decimal PriceTon)> pricings)
    {
        if (pricings.Count == 0)
            return Error.Validation("ChannelPricing.EmptyList", "At least one pricing must be provided");

        var hasDuplicates = pricings
            .GroupBy(p => (p.AdFormat, p.PriceType))
            .Any(g => g.Count() > 1);

        if (hasDuplicates)
            return Error.Validation("ChannelPricing.DuplicatePricing", "Duplicate pricing entries found in request");

        if (pricings.Any(p => p.PriceTon <= 0))
            return Error.Validation("ChannelPricing.InvalidPrice", "All prices must be greater than zero");

        var existingPricings = await dbContext.ChannelPricings
            .Where(p => p.ChannelId == channelId)
            .ToListAsync();

        var hasConflicts = pricings
            .Any(p => existingPricings.Any(e => e.AdFormat == p.AdFormat && e.PriceType == p.PriceType));

        if (hasConflicts)
            return Error.Conflict("ChannelPricing.AlreadyExists", "Some pricing entries already exist for this channel");

        var newPricings = pricings
            .Select(p => ChannelPricing.Create(channelId, p.AdFormat, p.PriceType, p.PriceTon))
            .ToList();

        dbContext.ChannelPricings.AddRange(newPricings);
        await dbContext.SaveChangesAsync();

        return newPricings;
    }

    public async Task<ErrorOr<ChannelPricing>> GetByIdAsync(Guid id)
    {
        var pricing = await dbContext.ChannelPricings
            .Include(p => p.Channel)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pricing is null)
            return Error.NotFound("ChannelPricing.NotFound", "Pricing not found");

        return pricing;
    }

    public async Task<ErrorOr<List<ChannelPricing>>> GetByChannelIdAsync(Guid channelId)
    {
        var pricings = await dbContext.ChannelPricings
            .Where(p => p.ChannelId == channelId)
            .ToListAsync();

        return pricings;
    }

    public async Task<ErrorOr<ChannelPricing>> UpdateAsync(
        Guid id,
        AdFormatType adFormat,
        PriceType priceType,
        decimal priceTon)
    {
        var pricing = await dbContext.ChannelPricings
            .AsTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pricing is null)
            return Error.NotFound("ChannelPricing.NotFound", "Pricing not found");

        if (priceTon <= 0)
            return Error.Validation("ChannelPricing.InvalidPrice", "Price must be greater than zero");

        var duplicate = await dbContext.ChannelPricings
            .FirstOrDefaultAsync(p => p.ChannelId == pricing.ChannelId 
                                      && p.AdFormat == adFormat 
                                      && p.PriceType == priceType 
                                      && p.Id != id);

        if (duplicate is not null)
            return Error.Conflict("ChannelPricing.AlreadyExists", "Pricing for this format and type already exists");

        pricing.Update(adFormat, priceType, priceTon);
        await dbContext.SaveChangesAsync();

        return pricing;
    }

    public async Task<ErrorOr<bool>> DeleteAsync(Guid id)
    {
        var pricing = await dbContext.ChannelPricings
            .AsTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pricing is null)
            return Error.NotFound("ChannelPricing.NotFound", "Pricing not found");

        var totalCount = await dbContext.ChannelPricings
            .CountAsync(p => p.ChannelId == pricing.ChannelId);

        if (totalCount <= 1)
            return Error.Validation("ChannelPricing.CannotDeleteLast", "Cannot delete the last pricing. Channel must have at least one pricing.");

        dbContext.ChannelPricings.Remove(pricing);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<ErrorOr<bool>> DeleteByChannelIdAsync(Guid channelId)
    {
        var pricings = await dbContext.ChannelPricings
            .AsTracking()
            .Where(p => p.ChannelId == channelId)
            .ToListAsync();

        dbContext.ChannelPricings.RemoveRange(pricings);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<ErrorOr<bool>> DeleteByChannelAndTypeAsync(Guid channelId, AdFormatType adFormat, PriceType priceType)
    {
        var pricing = await dbContext.ChannelPricings
            .AsTracking()
            .FirstOrDefaultAsync(p => p.ChannelId == channelId && p.AdFormat == adFormat && p.PriceType == priceType);

        if (pricing is null)
            return Error.NotFound("ChannelPricing.NotFound", "Pricing not found");

        var totalCount = await dbContext.ChannelPricings
            .CountAsync(p => p.ChannelId == channelId);

        if (totalCount <= 1)
            return Error.Validation("ChannelPricing.CannotDeleteLast", "Cannot delete the last pricing. Channel must have at least one pricing.");

        dbContext.ChannelPricings.Remove(pricing);
        await dbContext.SaveChangesAsync();

        return true;
    }
}

