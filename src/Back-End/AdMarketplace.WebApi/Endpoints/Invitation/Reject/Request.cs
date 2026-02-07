namespace AdMarketplace.Endpoints.Invitation.Reject;

public class Request
{
    public Guid Id { get; set; }
    public string? Feedback { get; set; }
}
