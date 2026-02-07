using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Application.GetByCampaign;

public class Response
{
    public required List<ApplicationItem> Applications { get; set; }
}

public class ApplicationItem
{
    public required Guid Id { get; set; }
    public required Guid ChannelId { get; set; }
    public required string ChannelTitle { get; set; }
    public required AdFormatType ProposedAdFormat { get; set; }
    public required PriceType ProposedPriceType { get; set; }
    public required decimal ProposedPriceTon { get; set; }
    public DateTimeOffset? ProposedPostingTime { get; set; }
    public string? Message { get; set; }
    public ApplicationStatusType Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
