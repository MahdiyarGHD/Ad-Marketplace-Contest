using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Channel.Create;

public class Request
{
    public required long ChatId { get; set; }
    public required Guid CategoryId { get; set; }
    public required List<PricingRequest> Pricings { get; set; }
}

public class PricingRequest
{
    public required AdFormatType AdFormat { get; set; }
    public required PriceType PriceType { get; set; }
    public required decimal PriceTon { get; set; }
}
