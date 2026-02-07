using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface ICampaignInvitationService
{
    Task<ErrorOr<CampaignInvitation>> CreateAsync(
        Guid campaignId,
        Guid channelId,
        AdFormatType proposedAdFormat,
        PriceType proposedPriceType,
        decimal proposedPriceTon,
        DateTimeOffset? proposedPostingTime = null,
        string? message = null);

    Task<ErrorOr<CampaignInvitation>> GetByIdAsync(Guid id);
    Task<ErrorOr<List<CampaignInvitation>>> GetByCampaignIdAsync(Guid campaignId, int skip, int take);
    Task<ErrorOr<List<CampaignInvitation>>> GetByChannelIdAsync(Guid channelId, int skip, int take);
    Task<ErrorOr<List<CampaignInvitation>>> GetByStatusAsync(
        Guid channelId,
        InvitationStatusType status,
        int skip,
        int take);

    Task<ErrorOr<CampaignInvitation>> AcceptAsync(Guid id, Guid channelOwnerId);
    Task<ErrorOr<CampaignInvitation>> RejectAsync(Guid id, Guid channelOwnerId, string? reason = null);
    Task<ErrorOr<CampaignInvitation>> WithdrawAsync(Guid id, Guid advertiserId);
}
