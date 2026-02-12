using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.ChannelApplication.CounterOffer;

public class Request
{
    public Guid Id { get; set; }
    public required AdFormatType AdFormat { get; set; }
    public required PriceType PriceType { get; set; }
    public required decimal PriceTon { get; set; }
    public DateTimeOffset? PostingTime { get; set; }
    public string? Message { get; set; }
}
