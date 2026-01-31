using AdMarketplace.Domain.Contracts.Responses;
using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Channel.MyChannels;

public class Endpoint(IChannelService channelService, IUserService userService)
    : EndpointWithoutRequest<ErrorOr<List<Response>>>
{
    public override void Configure()
    {
        Get("/api/channels/my");
    }

    public override async Task<ErrorOr<List<Response>>> ExecuteAsync(CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await channelService.GetByOwnerIdAsync(userResult.Value.Id);

        if (result.IsError)
            return result.Errors;

        return result.Value.Select(c => new Response
        {
            Id = c.Id,
            ChatId = c.ChatId,
            Title = c.Title,
            Status = c.Status,
            AverageViews = c.AverageViews,
            Category = new CategoryResponseDto
            {
                Id = c.Category?.Id ?? Guid.Empty,
                Name = c.Category?.Name ?? string.Empty,
                Icon = c.Category?.Icon,
            },
            Pricings = c.Pricings.Select(p => new ChannelPricingResponseContract
            {
                Id = p.Id,
                AdFormat = p.AdFormat,
                PriceType = p.PriceType,
                PriceTon = p.PriceTon
            }).ToList()
        }).ToList();
    }
}
