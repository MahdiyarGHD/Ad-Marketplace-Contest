using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface IDealService
{
    Task<ErrorOr<Deal>> CreateAsync(
        Guid? campaignId,
        Guid? applicationId,
        Guid? invitationId,
        Guid? channelApplicationId,
        Guid channelId,
        Guid advertiserId,
        decimal amountTon,
        AdFormatType adFormat,
        PriceType priceType,
        DateTimeOffset? scheduledPostTime = null,
        string? escrowWalletAddress = null);

    Task<ErrorOr<Deal>> GetByIdAsync(Guid id);
    Task<ErrorOr<Deal>> GetByApplicationIdAsync(Guid applicationId);
    Task<ErrorOr<List<Deal>>> GetByAdvertiserIdAsync(Guid advertiserId, int skip, int take);
    Task<ErrorOr<List<Deal>>> GetByChannelIdAsync(Guid channelId, int skip, int take);
    Task<ErrorOr<List<Deal>>> GetByCampaignIdAsync(Guid campaignId, int skip, int take);
    Task<ErrorOr<List<Deal>>> GetByStatusAsync(DealStatusType status, int skip, int take);
    Task<ErrorOr<List<Deal>>> GetExpiredDealsAsync();

    Task<ErrorOr<Deal>> FundEscrowAsync(Guid id, Guid advertiserId, string transactionHash, string walletAddress);
    Task<ErrorOr<Deal>> SubmitDraftAsync(Guid id, Guid channelOwnerId, long messageId);
    Task<ErrorOr<Deal>> ApproveDraftAsync(Guid id, Guid advertiserId);
    Task<ErrorOr<Deal>> RejectDraftAsync(Guid id, Guid advertiserId, string feedback);
    Task<ErrorOr<Deal>> MarkAsPostedAsync(Guid id, long messageId);
    Task<ErrorOr<Deal>> ReleaseFundsAsync(Guid id);
    Task<ErrorOr<Deal>> RefundAsync(Guid id);
    Task<ErrorOr<Deal>> ResolveDisputeAsync(Guid id, DisputeResolutionType resolution);
    Task<ErrorOr<Deal>> UpdateStatusAsync(Guid id, DealStatusType status);
}
