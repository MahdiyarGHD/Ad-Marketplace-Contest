namespace AdMarketplace.Endpoints.Deal.Create;

public class Request
{
    public required Guid CampaignId { get; set; }
    public required Guid ApplicationId { get; set; }
    public required Guid ChannelId { get; set; }
    public string? EscrowWalletAddress { get; set; }
}
