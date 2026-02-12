using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.ChannelApplication.CounterOffer;

public class Endpoint(
    IChannelApplicationService channelApplicationService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/channel-applications/{Id}/counter-offer");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await channelApplicationService.CounterOfferAsync(
            req.Id,
            userResult.Value.Id,
            req.AdFormat,
            req.PriceType,
            req.PriceTon,
            req.PostingTime,
            req.Message);

        if (result.IsError)
            return result.Errors;

        var application = result.Value;

        return new Response
        {
            Id = application.Id,
            Status = application.Status,
            CounterAdFormat = application.CounterAdFormat,
            CounterPriceType = application.CounterPriceType,
            CounterPriceTon = application.CounterPriceTon,
            CounterPostingTime = application.CounterPostingTime,
            CounterMessage = application.CounterMessage,
            UpdatedAt = application.UpdatedAt
        };
    }
}
