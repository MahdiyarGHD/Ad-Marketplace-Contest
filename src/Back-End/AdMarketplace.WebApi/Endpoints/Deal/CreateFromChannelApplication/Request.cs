namespace AdMarketplace.Endpoints.Deal.CreateFromChannelApplication;

public class Request
{
    public required Guid ChannelApplicationId { get; set; }
    public required Guid ChannelId { get; set; }
    public string? EscrowWalletAddress { get; set; }
}
