using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Deal.ResolveDispute;

public class Response
{
    public required Guid Id { get; set; }
    public DealStatusType Status { get; set; }
    public DisputeResolutionType Resolution { get; set; }
    public DateTimeOffset? FundsReleasedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
