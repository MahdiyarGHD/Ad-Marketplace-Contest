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
}
