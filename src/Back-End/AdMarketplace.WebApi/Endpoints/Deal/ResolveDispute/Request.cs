using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Deal.ResolveDispute;

public class Request
{
    public Guid Id { get; set; }
    public DisputeResolutionType Resolution { get; set; }
}
