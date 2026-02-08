using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Invitation.GetByChannel;

public class Response
{
    public required List<InvitationItem> Invitations { get; set; }
}

public class InvitationItem
{
    public required Guid Id { get; set; }
    public required Guid CampaignId { get; set; }
    public required string CampaignTitle { get; set; }
    public required Guid ChannelId { get; set; }
    public required AdFormatType ProposedAdFormat { get; set; }
    public required PriceType ProposedPriceType { get; set; }
    public required decimal ProposedPriceTon { get; set; }
    public DateTimeOffset? ProposedPostingTime { get; set; }
    public string? Message { get; set; }
    public InvitationStatusType Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
