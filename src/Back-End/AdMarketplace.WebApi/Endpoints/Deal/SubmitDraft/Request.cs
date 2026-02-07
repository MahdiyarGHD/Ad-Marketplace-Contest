namespace AdMarketplace.Endpoints.Deal.SubmitDraft;

public class Request
{
    public Guid Id { get; set; }
    public required long DraftMessageId { get; set; }
}
