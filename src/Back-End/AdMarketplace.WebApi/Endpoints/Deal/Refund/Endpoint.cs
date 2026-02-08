using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Deal.Refund;

public class Endpoint(
    IDealService dealService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/deals/{Id}/refund");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var dealResult = await dealService.GetByIdAsync(req.Id);
        if (dealResult.IsError)
            return dealResult.Errors;

        if (dealResult.Value.AdvertiserId != userResult.Value.Id)
            return Error.Forbidden("Deal.Forbidden", "You don't have permission to refund this deal");

        var result = await dealService.RefundAsync(req.Id);
        if (result.IsError)
            return result.Errors;

        var deal = result.Value;

        return new Response
        {
            Id = deal.Id,
            Status = deal.Status,
            FundsReleasedAt = deal.FundsReleasedAt,
            UpdatedAt = deal.UpdatedAt
        };
    }
}
