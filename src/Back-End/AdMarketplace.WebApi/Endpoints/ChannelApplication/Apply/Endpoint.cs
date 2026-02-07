using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.ChannelApplication.Apply;

public class Endpoint(
    IChannelApplicationService channelApplicationService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/channel-applications");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await channelApplicationService.CreateAsync(
            channelId: req.ChannelId,
            advertiserId: userResult.Value.Id,
            proposedAdFormat: req.ProposedAdFormat,
            proposedPriceType: req.ProposedPriceType,
            proposedPriceTon: req.ProposedPriceTon,
            proposedPostingTime: req.ProposedPostingTime,
            message: req.Message);

        if (result.IsError)
            return result.Errors;

        var application = result.Value;

        return new Response
        {
            Id = application.Id,
            ChannelId = application.ChannelId,
            AdvertiserId = application.AdvertiserId,
            ProposedAdFormat = application.ProposedAdFormat,
            ProposedPriceType = application.ProposedPriceType,
            ProposedPriceTon = application.ProposedPriceTon,
            Status = application.Status,
            CreatedAt = application.CreatedAt
        };
    }
}
