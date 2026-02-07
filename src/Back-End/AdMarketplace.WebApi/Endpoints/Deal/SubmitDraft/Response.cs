using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Deal.SubmitDraft;

public class Response
{
    public required Guid Id { get; set; }
    public DealStatusType Status { get; set; }
    public DraftStatusType DraftStatus { get; set; }
    public long? DraftMessageId { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
