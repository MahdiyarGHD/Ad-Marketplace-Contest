using AdMarketplace.Domain.Types;

namespace AdMarketplace.Domain.Contracts.Responses;

public class ChannelPricingResponseContract
{
    public Guid Id { get; set; }
    public AdFormatType AdFormat { get; set; }
    public PriceType PriceType { get; set; }
    public decimal PriceTon { get; set; }
}

