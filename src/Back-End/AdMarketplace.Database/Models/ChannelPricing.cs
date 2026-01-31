using AdMarketplace.Domain.Types;
using HazelApp.Domain.Common.Schemas;

namespace AdMarketplace.Database.Models;

public class ChannelPricing : IDateTimeSchema
{
    public Guid Id { get; private set; }
    public Guid ChannelId { get; private set; }
    public AdFormatType AdFormat { get; private set; }
    public PriceType PriceType { get; private set; }
    public decimal PriceTon { get; private set; }

    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public Channel Channel { get; private set; } = null!;

    public static ChannelPricing Create(
        Guid channelId,
        AdFormatType adFormat,
        PriceType priceType,
        decimal priceTon)
    {
        return new ChannelPricing
        {
            Id = Guid.CreateVersion7(),
            ChannelId = channelId,
            AdFormat = adFormat,
            PriceType = priceType,
            PriceTon = priceTon,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Update(
        AdFormatType adFormat,
        PriceType priceType,
        decimal priceTon)
    {
        AdFormat = adFormat;
        PriceType = priceType;
        PriceTon = priceTon;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}