using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services.ChannelServices;

public class ChannelService(AdMarketDbContext dbContext, ICategoryService categoryService) : IChannelService
{
    public async Task<ErrorOr<Channel>> CreateAsync(
        long chatId,
        string title,
        string? username,
        string? description,
        Guid ownerId,
        Guid categoryId)
    {
        var existingChannel = await dbContext.Channels
            .FirstOrDefaultAsync(c => c.ChatId == chatId);

        if (existingChannel is not null)
            return Error.Conflict("Channel.AlreadyExists", "Channel with this Telegram ID already exists");

        var categoryResult = await categoryService.GetByIdAsync(categoryId);
        if (categoryResult.IsError)
            return categoryResult.Errors;

        var channel = Channel.Create(
            chatId: chatId,
            title: title,
            username: username,
            description: description,
            ownerId: ownerId,
            categoryId: categoryId);

        dbContext.Channels.Add(channel);
        await dbContext.SaveChangesAsync();

        return channel;
    }

    public async Task<ErrorOr<Channel>> GetByIdAsync(Guid id)
    {
        var channel = await dbContext.Channels
            .Include(c => c.Owner)
            .Include(c => c.Category)
            .Include(c => c.Pricings)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (channel is null)
            return Error.NotFound("Channel.NotFound", "Channel not found");

        return channel;
    }

    public async Task<ErrorOr<Channel>> GetByTelegramIdAsync(long telegramChannelId)
    {
        var channel = await dbContext.Channels
            .Include(c => c.Owner)
            .Include(c => c.Category)
            .Include(c => c.Pricings)
            .FirstOrDefaultAsync(c => c.ChatId == telegramChannelId);

        if (channel is null)
            return Error.NotFound("Channel.NotFound", "Channel not found");

        return channel;
    }

    public async Task<ErrorOr<List<Channel>>> GetByOwnerIdAsync(Guid ownerId)
    {
        var channels = await dbContext.Channels
            .Include(c => c.Owner)
            .Include(c => c.Category)
            .Include(c => c.Pricings)
            .Where(c => c.OwnerId == ownerId)
            .ToListAsync();

        return channels;
    }

    public async Task<ErrorOr<List<Channel>>> GetAllActiveAsync(int skip, int take)
    {
        var channels = await dbContext.Channels
            .Include(c => c.Owner)
            .Include(c => c.Category)
            .Include(c => c.Pricings)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return channels;
    }

    public async Task<ErrorOr<List<Channel>>> GetByCategoryIdAsync(Guid categoryId, int skip, int take)
    {
        var channels = await dbContext.Channels
            .Include(c => c.Owner)
            .Include(c => c.Category)
            .Include(c => c.Pricings)
            .Where(c => c.CategoryId == categoryId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return channels;
    }

    public async Task<ErrorOr<List<Channel>>> SearchAsync(
        Guid? categoryId = null,
        int? minSubscribers = null,
        int? maxSubscribers = null,
        int? minAverageViews = null,
        AdFormatType? adFormat = null,
        PriceType? priceType = null,
        decimal? maxPrice = null,
        string? language = null,
        int skip = 0,
        int take = 20)
    {
        var query = dbContext.Channels
            .Include(c => c.Owner)
            .Include(c => c.Category)
            .Include(c => c.Pricings)
            .Where(c => c.Status == ChannelStatusType.Ready)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(c => c.CategoryId == categoryId.Value);

        if (minSubscribers.HasValue)
            query = query.Where(c => c.SubscriberCount >= minSubscribers.Value);

        if (maxSubscribers.HasValue)
            query = query.Where(c => c.SubscriberCount <= maxSubscribers.Value);

        if (minAverageViews.HasValue)
            query = query.Where(c => c.AverageViews >= minAverageViews.Value);

        if (adFormat.HasValue)
            query = query.Where(c => c.Pricings.Any(p => p.AdFormat == adFormat.Value));

        if (priceType.HasValue)
            query = query.Where(c => c.Pricings.Any(p => p.PriceType == priceType.Value));

        if (maxPrice.HasValue)
            query = query.Where(c => c.Pricings.Any(p => p.PriceTon <= maxPrice.Value));

        var channels = await query
            .OrderByDescending(c => c.SubscriberCount)
            .ToListAsync();

        if (!string.IsNullOrEmpty(language))
            channels = channels
                .Where(c => c.LanguageDistributionJson != null &&
                            c.LanguageDistributionJson.Any(l =>
                                l.Language.Equals(language, StringComparison.OrdinalIgnoreCase)))
                .ToList();

        return channels
            .Skip(skip)
            .Take(take)
            .ToList();
    }

    public async Task<ErrorOr<Channel>> UpdateAsync(
        Guid id,
        Guid ownerId,
        Guid categoryId,
        List<(AdFormatType AdFormat, PriceType PriceType, decimal PriceTon)> pricings)
    {
        var channel = await dbContext.Channels
            .Include(c => c.Category)
            .Include(c => c.Pricings)
            .AsTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (channel is null)
            return Error.NotFound("Channel.NotFound", "Channel not found");

        if (channel.OwnerId != ownerId)
            return Error.Forbidden("Channel.NotOwner", "You are not the owner of this channel");

        var categoryResult = await categoryService.GetByIdAsync(categoryId);
        if (categoryResult.IsError)
            return categoryResult.Errors;

        if (pricings.Count == 0)
            return Error.Validation("Channel.NoPricings", "Channel must have at least one pricing");

        if (pricings.Any(p => p.PriceTon <= 0))
            return Error.Validation("ChannelPricing.InvalidPrice", "All prices must be greater than zero");

        var hasDuplicates = pricings
            .GroupBy(p => (p.AdFormat, p.PriceType))
            .Any(g => g.Count() > 1);

        if (hasDuplicates)
            return Error.Validation("ChannelPricing.DuplicatePricing", "Duplicate pricing entries found");

        channel.UpdateCategory(categoryId);

        dbContext.ChannelPricings.RemoveRange(channel.Pricings);

        foreach (var p in pricings)
        {
            channel.Pricings.Add(ChannelPricing.Create(channel.Id, p.AdFormat, p.PriceType, p.PriceTon));
        }

        await dbContext.SaveChangesAsync();

        return channel;
    }

    public async Task<ErrorOr<bool>> SetStatusAsync(Guid id, Guid ownerId, ChannelStatusType status)
    {
        var channel = await dbContext.Channels
            .AsTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (channel is null)
            return Error.NotFound("Channel.NotFound", "Channel not found");

        if (channel.OwnerId != ownerId)
            return Error.Forbidden("Channel.NotOwner", "You are not the owner of this channel");

        channel.SetStatus(status);
        await dbContext.SaveChangesAsync();

        return true;
    }
}
