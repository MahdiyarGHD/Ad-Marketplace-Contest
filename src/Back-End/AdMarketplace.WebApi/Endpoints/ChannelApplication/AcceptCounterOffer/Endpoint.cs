using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.ChannelApplication.AcceptCounterOffer;

public class Endpoint(
    IChannelApplicationService channelApplicationService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/channel-applications/{Id}/accept-counter-offer");
        Description(d => d.ClearDefaultAccepts());
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await channelApplicationService.AcceptCounterOfferAsync(req.Id, userResult.Value.Id);

        if (result.IsError)
            return result.Errors;

        var application = result.Value;

        return new Response
        {
            Id = application.Id,
            Status = application.Status,
            UpdatedAt = application.UpdatedAt
        };
    }
}
