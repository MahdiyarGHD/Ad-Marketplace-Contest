using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.ChannelApplication.CounterOffer;

public class Response
{
    public required Guid Id { get; set; }
    public ApplicationStatusType Status { get; set; }
    public AdFormatType? CounterAdFormat { get; set; }
    public PriceType? CounterPriceType { get; set; }
    public decimal? CounterPriceTon { get; set; }
    public DateTimeOffset? CounterPostingTime { get; set; }
    public string? CounterMessage { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
