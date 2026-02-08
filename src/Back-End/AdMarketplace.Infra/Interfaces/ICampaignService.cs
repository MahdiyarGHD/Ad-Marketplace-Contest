using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Types;
using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface ICampaignService
{
    Task<ErrorOr<Campaign>> CreateAsync(
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
        DateTimeOffset? applicationDeadline = null);

    Task<ErrorOr<Campaign>> GetByIdAsync(Guid id);
    Task<ErrorOr<List<Campaign>>> GetByAdvertiserIdAsync(Guid advertiserId, int skip, int take);
    Task<ErrorOr<List<Campaign>>> GetActiveAsync(int skip, int take);
    Task<ErrorOr<List<Campaign>>> GetByCategoryIdAsync(Guid categoryId, int skip, int take);
    Task<ErrorOr<List<Campaign>>> SearchAsync(
        string? keyword = null,
        Guid? categoryId = null,
        decimal? minBudget = null,
        decimal? maxBudget = null,
        CampaignStatusType? status = null,
        AdFormatType? adFormat = null,
        PriceType? priceType = null,
        int skip = 0,
        int take = 20);

    Task<ErrorOr<Campaign>> UpdateAsync(
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
        DateTimeOffset? applicationDeadline = null);

    Task<ErrorOr<Campaign>> UpdateStatusAsync(Guid id, Guid advertiserId, CampaignStatusType status);
    Task<ErrorOr<bool>> DeleteAsync(Guid id, Guid advertiserId);
}
