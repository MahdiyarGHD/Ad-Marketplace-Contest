using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Channel.Create;

public class Response
{
    public required Guid Id { get; set; }
    public required long ChatId { get; set; }
    public required string Title { get; set; }
    public ChannelStatusType Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public required List<PricingResponse> Pricings { get; set; }
}

public class PricingResponse
{
    public required Guid Id { get; set; }
    public required AdFormatType AdFormat { get; set; }
    public required PriceType PriceType { get; set; }
    public required decimal PriceTon { get; set; }
}
