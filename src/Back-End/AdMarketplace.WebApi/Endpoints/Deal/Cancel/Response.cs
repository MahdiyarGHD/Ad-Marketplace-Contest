using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Deal.Cancel;

public class Response
{
    public required Guid Id { get; set; }
    public DealStatusType Status { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
