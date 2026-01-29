using AdMarketplace.Database.Models;
using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface IUserChannelConnectionService
{
    Task<ErrorOr<UserChannelConnection>> CreateOrTouchAsync(string title, long userId, long chatId);
    Task<ErrorOr<List<UserChannelConnection>>> GetRecentByUserIdAsync(Guid userId, DateTimeOffset sinceUtc);
}

