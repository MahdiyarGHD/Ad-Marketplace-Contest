using AdMarketplace.Domain.Contracts.Responses;
using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Channel.Update;

public class Endpoint(IChannelService channelService, IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Put("/api/channels/{Id}");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await channelService.UpdateAsync(
            id: req.Id,
            ownerId: userResult.Value.Id,
            categoryId: req.CategoryId,
            pricings: req.Pricings.Select(p => (p.AdFormat, p.PriceType, p.PriceTon)).ToList());

        if (result.IsError)
            return result.Errors;

        return new Response
        {
            Id = result.Value.Id,
            TelegramChannelId = result.Value.ChatId,
            Title = result.Value.Title,
            Username = result.Value.Username,
            Description = result.Value.Description,
            SubscriberCount = result.Value.SubscriberCount,
            AverageViews = result.Value.AverageViews,
            LanguageDistributionJson = result.Value.LanguageDistributionJson,
            CategoryId = result.Value.CategoryId,
            CategoryName = result.Value.Category?.Name,
            UpdatedAt = result.Value.UpdatedAt ?? DateTimeOffset.UtcNow,
            Pricings = result.Value.Pricings.Select(p => new ChannelPricingResponseContract
            {
                Id = p.Id,
                AdFormat = p.AdFormat,
                PriceType = p.PriceType,
                PriceTon = p.PriceTon
            }).ToList()
        };
    }
}
