using AdMarketplace.Domain.Types;
using Microsoft.AspNetCore.Mvc;

namespace AdMarketplace.Endpoints.Channel.Update;

public class Request
{
    [FromRoute]
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public List<PricingUpdateRequest> Pricings { get; set; }
}

public class PricingUpdateRequest
{
    public required AdFormatType AdFormat { get; set; }
    public required PriceType PriceType { get; set; }
    public required decimal PriceTon { get; set; }
}
