using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Deal.ResolveDispute;

public class Endpoint(
    IDealService dealService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/deals/{Id}/resolve-dispute");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var dealResult = await dealService.GetByIdAsync(req.Id);
        if (dealResult.IsError)
            return dealResult.Errors;

        var deal = dealResult.Value;

        if (deal.AdvertiserId != userResult.Value.Id && deal.Channel.OwnerId != userResult.Value.Id)
            return Error.Forbidden("Deal.Forbidden", "You don't have permission to resolve this dispute");

        var result = await dealService.ResolveDisputeAsync(req.Id, req.Resolution);
        if (result.IsError)
            return result.Errors;

        var resolved = result.Value;

        return new Response
        {
            Id = resolved.Id,
            Status = resolved.Status,
            Resolution = req.Resolution,
            FundsReleasedAt = resolved.FundsReleasedAt,
            UpdatedAt = resolved.UpdatedAt
        };
    }
}
