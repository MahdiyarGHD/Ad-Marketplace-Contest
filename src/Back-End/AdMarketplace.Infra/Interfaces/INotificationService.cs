using AdMarketplace.Domain.Types;

namespace AdMarketplace.Infra.Interfaces;

public interface INotificationService
{
    Task NotifyDealCreatedAsync(Guid dealId, CancellationToken ct = default);
    Task NotifyEscrowFundedAsync(Guid dealId, CancellationToken ct = default);
    Task NotifyDraftSubmittedAsync(Guid dealId, CancellationToken ct = default);
    Task NotifyDraftApprovedAsync(Guid dealId, CancellationToken ct = default);
    Task NotifyDraftRejectedAsync(Guid dealId, string feedback, CancellationToken ct = default);
    Task NotifyDealPostedAsync(Guid dealId, CancellationToken ct = default);
    Task NotifyDealCompletedAsync(Guid dealId, CancellationToken ct = default);
    Task NotifyDealRefundedAsync(Guid dealId, CancellationToken ct = default);
    Task NotifyDealCancelledAsync(Guid dealId, CancellationToken ct = default);
    Task NotifyDealDisputedAsync(Guid dealId, CancellationToken ct = default);
    Task NotifyDisputeResolvedAsync(Guid dealId, DisputeResolutionType resolution, CancellationToken ct = default);
    Task NotifyPostDeletedAsync(Guid dealId, CancellationToken ct = default);

    Task NotifyChannelApplicationReceivedAsync(Guid applicationId, CancellationToken ct = default);
    Task NotifyChannelApplicationAcceptedAsync(Guid applicationId, CancellationToken ct = default);
    Task NotifyChannelApplicationRejectedAsync(Guid applicationId, string? reason, CancellationToken ct = default);
    Task NotifyChannelApplicationCounterOfferAsync(Guid applicationId, CancellationToken ct = default);

    Task NotifyCampaignApplicationReceivedAsync(Guid applicationId, CancellationToken ct = default);
    Task NotifyCampaignApplicationAcceptedAsync(Guid applicationId, CancellationToken ct = default);
    Task NotifyCampaignApplicationRejectedAsync(Guid applicationId, string? reason, CancellationToken ct = default);
    Task NotifyCampaignApplicationCounterOfferAsync(Guid applicationId, CancellationToken ct = default);

    Task NotifyCampaignInvitationReceivedAsync(Guid invitationId, CancellationToken ct = default);
    Task NotifyCampaignInvitationAcceptedAsync(Guid invitationId, CancellationToken ct = default);
    Task NotifyCampaignInvitationRejectedAsync(Guid invitationId, string? reason, CancellationToken ct = default);
    Task NotifyCampaignInvitationWithdrawnAsync(Guid invitationId, CancellationToken ct = default);
}
