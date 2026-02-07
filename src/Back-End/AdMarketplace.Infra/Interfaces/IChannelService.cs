using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Types;
using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface IChannelService
{
    Task<ErrorOr<Channel>> CreateAsync(
        long chatId,
        string title,
        string? username,
        string? description,
        Guid ownerId,
        Guid categoryId);

    Task<ErrorOr<Channel>> GetByIdAsync(Guid id);
    Task<ErrorOr<Channel>> GetByTelegramIdAsync(long telegramChannelId);
    Task<ErrorOr<List<Channel>>> GetByOwnerIdAsync(Guid ownerId);
    Task<ErrorOr<List<Channel>>> GetAllActiveAsync(int skip, int take);
    Task<ErrorOr<List<Channel>>> GetByCategoryIdAsync(Guid categoryId, int skip, int take);

    Task<ErrorOr<List<Channel>>> SearchAsync(
        Guid? categoryId = null,
        int? minSubscribers = null,
        int? maxSubscribers = null,
        int? minAverageViews = null,
        AdFormatType? adFormat = null,
        PriceType? priceType = null,
        decimal? maxPrice = null,
        string? language = null,
        int skip = 0,
        int take = 20);

    Task<ErrorOr<Channel>> UpdateAsync(
        Guid id,
        Guid ownerId,
        Guid categoryId,
        List<(AdFormatType AdFormat, PriceType PriceType, decimal PriceTon)> pricings);

    Task<ErrorOr<bool>> SetStatusAsync(Guid id, Guid ownerId, ChannelStatusType status);
}
