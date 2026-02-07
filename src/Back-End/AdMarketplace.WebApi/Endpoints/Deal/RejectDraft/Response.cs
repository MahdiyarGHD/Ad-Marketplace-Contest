using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Deal.RejectDraft;

public class Response
{
    public required Guid Id { get; set; }
    public DealStatusType Status { get; set; }
    public DraftStatusType DraftStatus { get; set; }
    public string? AdvertiserFeedback { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
