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

    Task<ErrorOr<Channel>> UpdateAsync(
        Guid id,
        string title,
        string? description,
        int subscriberCount,
        int averageViews,
        List<LanguageDistributionContract>? languageDistributionJson,
        Guid categoryId,
        Guid ownerId);

    Task<ErrorOr<bool>> SetStatusAsync(Guid id, Guid ownerId, ChannelStatusType status);
}
