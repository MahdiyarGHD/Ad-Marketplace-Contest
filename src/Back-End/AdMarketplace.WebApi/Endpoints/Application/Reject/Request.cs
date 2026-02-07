namespace AdMarketplace.Endpoints.Application.Reject;

public class Request
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }
}
