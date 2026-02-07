using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface ICampaignApplicationService
{
    Task<ErrorOr<CampaignApplication>> CreateAsync(
        Guid campaignId,
        Guid channelId,
        AdFormatType proposedAdFormat,
        PriceType proposedPriceType,
        decimal proposedPriceTon,
        DateTimeOffset? proposedPostingTime = null,
        string? message = null);

    Task<ErrorOr<CampaignApplication>> GetByIdAsync(Guid id);
    Task<ErrorOr<List<CampaignApplication>>> GetByCampaignIdAsync(Guid campaignId, int skip, int take);
    Task<ErrorOr<List<CampaignApplication>>> GetByChannelIdAsync(Guid channelId, int skip, int take);
    Task<ErrorOr<List<CampaignApplication>>> GetByStatusAsync(
        Guid campaignId,
        ApplicationStatusType status,
        int skip,
        int take);

    Task<ErrorOr<CampaignApplication>> AcceptAsync(Guid id, Guid advertiserId);
    Task<ErrorOr<CampaignApplication>> RejectAsync(Guid id, Guid advertiserId, string? reason = null);
    Task<ErrorOr<CampaignApplication>> WithdrawAsync(Guid id, Guid channelOwnerId);
}
