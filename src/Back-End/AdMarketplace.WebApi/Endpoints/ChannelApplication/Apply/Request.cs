using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.ChannelApplication.Apply;

public class Request
{
    public required Guid ChannelId { get; set; }
    public required AdFormatType ProposedAdFormat { get; set; }
    public required PriceType ProposedPriceType { get; set; }
    public required decimal ProposedPriceTon { get; set; }
    public DateTimeOffset? ProposedPostingTime { get; set; }
    public string? Message { get; set; }
}
