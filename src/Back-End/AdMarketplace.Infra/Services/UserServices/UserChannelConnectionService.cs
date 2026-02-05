using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdMarketplace.Infra.Services.UserServices;

public class UserChannelConnectionService(AdMarketDbContext dbContext, IUserService userService, ILogger<UserChannelConnectionService> logger) : IUserChannelConnectionService
{
    public async Task<ErrorOr<UserChannelConnection>> CreateOrTouchAsync(string title, long userId, long chatId)
    {
        var targetUserId = await userService.GetByUserIdAsync(userId);
        if (targetUserId.IsError)
            return targetUserId.Errors;
        
        var userGuid = targetUserId.Value.Id;
        var existing = await dbContext.UserChannelConnection
            .FirstOrDefaultAsync(x => x.UserId == userGuid && x.ChatId == chatId);

        if (existing is not null)
        {
            existing.Touch();
            await dbContext.SaveChangesAsync();
            return existing;
        }

        var link = UserChannelConnection.Create(title, userGuid, chatId);
        dbContext.UserChannelConnection.Add(link);
        await dbContext.SaveChangesAsync();
        
        logger.LogInformation("User {UserId} connected bot to channel {ChatId} - {Title} successfully.", userId, chatId, title);
        
        return link;
    }

    public async Task<ErrorOr<int>> RemoveChannelConnectionsAsync(long chatId)
    {
        var query = UserChannelConnection.ByChannel(dbContext.UserChannelConnection, chatId);
        var deletedCount = await query.ExecuteDeleteAsync();
        
        logger.LogInformation("Removed {Count} connection(s) for channel {ChatId}", deletedCount, chatId);
        
        return deletedCount;
    }

    public async Task<ErrorOr<List<UserChannelConnection>>> GetRecentByUserIdAsync(Guid userId, DateTimeOffset sinceUtc)
    {
        var links = await dbContext.UserChannelConnection
            .Where(x => x.UserId == userId)
            .Where(x => (x.UpdatedAt ?? x.CreatedAt) >= sinceUtc)
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ToListAsync();

        return links;
    }

    public async Task<ErrorOr<UserChannelConnection>> GetByUserAndChatIdAsync(Guid userId, long chatId)
    {
        var connection = await dbContext.UserChannelConnection
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ChatId == chatId);

        if (connection is null)
            return Error.NotFound("UserChannelConnection.NotFound", "No connection found between user and channel");

        return connection;
    }
}

