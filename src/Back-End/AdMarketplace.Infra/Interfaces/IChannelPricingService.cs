using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Types;
using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface IChannelPricingService
{
    Task<ErrorOr<ChannelPricing>> CreateAsync(
        Guid channelId,
        AdFormatType adFormat,
        PriceType priceType,
        decimal priceTon);

    Task<ErrorOr<List<ChannelPricing>>> CreateBulkAsync(
        Guid channelId,
        List<(AdFormatType AdFormat, PriceType PriceType, decimal PriceTon)> pricings);

    Task<ErrorOr<ChannelPricing>> GetByIdAsync(Guid id);
    Task<ErrorOr<List<ChannelPricing>>> GetByChannelIdAsync(Guid channelId);

    Task<ErrorOr<ChannelPricing>> UpdateAsync(
        Guid id,
        AdFormatType adFormat,
        PriceType priceType,
        decimal priceTon);

    Task<ErrorOr<bool>> DeleteAsync(Guid id);
    Task<ErrorOr<bool>> DeleteByChannelIdAsync(Guid channelId);
    Task<ErrorOr<bool>> DeleteByChannelAndTypeAsync(Guid channelId, AdFormatType adFormat, PriceType priceType);
}

