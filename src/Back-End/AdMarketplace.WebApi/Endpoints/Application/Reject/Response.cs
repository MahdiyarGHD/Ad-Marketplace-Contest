using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Application.Reject;

public class Response
{
    public required Guid Id { get; set; }
    public ApplicationStatusType Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
