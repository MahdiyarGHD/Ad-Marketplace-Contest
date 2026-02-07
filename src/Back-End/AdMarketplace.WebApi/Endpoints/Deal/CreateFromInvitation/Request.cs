namespace AdMarketplace.Endpoints.Deal.CreateFromInvitation;

public class Request
{
    public required Guid CampaignId { get; set; }
    public required Guid InvitationId { get; set; }
    public required Guid ChannelId { get; set; }
    public string? EscrowWalletAddress { get; set; }
}
