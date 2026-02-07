namespace AdMarketplace.Endpoints.Deal.RejectDraft;

public class Request
{
    public Guid Id { get; set; }
    public required string Feedback { get; set; }
}
