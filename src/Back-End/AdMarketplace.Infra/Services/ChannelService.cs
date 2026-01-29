using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services;

public class ChannelService(AdMarketDbContext dbContext) : IChannelService
{
    public async Task<ErrorOr<Channel>> CreateAsync(
        long telegramChannelId,
        string title,
        string? username,
        string? description,
        int subscriberCount,
        int averageViews,
        List<LanguageDistributionContract>? languageDistributionJson,
        Guid ownerId,
        Guid? categoryId)
    {
        var existingChannel = await dbContext.Channels
            .FirstOrDefaultAsync(c => c.ChatId == telegramChannelId);

        if (existingChannel is not null)
            return Error.Conflict("Channel.AlreadyExists", "Channel with this Telegram ID already exists");

        var channel = Channel.Create(
            telegramChannelId: telegramChannelId,
            title: title,
            username: username,
            description: description,
            subscriberCount: subscriberCount,
            averageViews: averageViews,
            languageDistributionJson: languageDistributionJson,
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
            .Where(c => c.OwnerId == ownerId)
            .ToListAsync();

        return channels;
    }

    public async Task<ErrorOr<List<Channel>>> GetAllActiveAsync(int skip, int take)
    {
        var channels = await dbContext.Channels
            .Include(c => c.Owner)
            .Include(c => c.Category)
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
            .Where(c => c.CategoryId == categoryId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return channels;
    }

    public async Task<ErrorOr<Channel>> UpdateAsync(
        Guid id,
        string title,
        string? description,
        int subscriberCount,
        int averageViews,
        List<LanguageDistributionContract>? languageDistributionJson,
        Guid? categoryId,
        Guid ownerId)
    {
        var channel = await dbContext.Channels.FirstOrDefaultAsync(c => c.Id == id);

        if (channel is null)
            return Error.NotFound("Channel.NotFound", "Channel not found");

        if (channel.OwnerId != ownerId)
            return Error.Forbidden("Channel.NotOwner", "You are not the owner of this channel");

        channel.Update(
            title: title,
            description: description,
            subscriberCount: subscriberCount,
            averageViews: averageViews,
            languageDistributionJson: languageDistributionJson,
            categoryId: categoryId);

        await dbContext.SaveChangesAsync();

        return channel;
    }

    public async Task<ErrorOr<bool>> SetStatusAsync(Guid id, Guid ownerId, ChannelStatusType status)
    {
        var channel = await dbContext.Channels.FirstOrDefaultAsync(c => c.Id == id);

        if (channel is null)
            return Error.NotFound("Channel.NotFound", "Channel not found");

        if (channel.OwnerId != ownerId)
            return Error.Forbidden("Channel.NotOwner", "You are not the owner of this channel");

        channel.SetStatus(status);
        await dbContext.SaveChangesAsync();

        return true;
    }
}
