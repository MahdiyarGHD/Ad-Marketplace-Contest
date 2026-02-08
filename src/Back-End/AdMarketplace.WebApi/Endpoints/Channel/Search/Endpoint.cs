using AdMarketplace.Domain.Contracts.Responses;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Channel.Search;

public class Endpoint(IChannelService channelService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/channels/search");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var result = await channelService.SearchAsync(
            keyword: req.Query,
            categoryId: req.CategoryId,
            minSubscribers: req.MinSubscribers,
            maxSubscribers: req.MaxSubscribers,
            minAverageViews: req.MinAverageViews,
            adFormat: req.AdFormat,
            priceType: req.PriceType,
            maxPrice: req.MaxPrice,
            language: req.Language,
            skip: req.Skip,
            take: req.Take);

        if (result.IsError)
            return result.Errors;

        return new Response
        {
            Channels = result.Value.Select(c => new ChannelResponseContract
            {
                Id = c.Id,
                ChatId = c.ChatId,
                Title = c.Title,
                Username = c.Username,
                Description = c.Description,
                SubscriberCount = c.SubscriberCount,
                AverageViews = c.AverageViews,
                LanguageDistributionJson = c.LanguageDistributionJson,
                Pricings = c.Pricings.Select(p => new ChannelPricingResponseContract
                {
                    Id = p.Id,
                    AdFormat = p.AdFormat,
                    PriceType = p.PriceType,
                    PriceTon = p.PriceTon
                }).ToList()
            }).ToList()
        };
    }
}
