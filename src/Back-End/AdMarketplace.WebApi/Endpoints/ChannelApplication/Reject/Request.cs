namespace AdMarketplace.Endpoints.ChannelApplication.Reject;

public class Request
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }
}
