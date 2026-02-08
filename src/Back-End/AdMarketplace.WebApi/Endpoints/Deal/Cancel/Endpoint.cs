using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Deal.Cancel;

public class Endpoint(
    IDealService dealService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/deals/{Id}/cancel");
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
            return Error.Forbidden("Deal.Forbidden", "You don't have permission to cancel this deal");

        if (deal.HasFundedEscrow)
            return Error.Validation("Deal.HasEscrow", "Cannot cancel a funded deal. Use refund instead.");

        var result = await dealService.UpdateStatusAsync(req.Id, Domain.Types.DealStatusType.Cancelled);
        if (result.IsError)
            return result.Errors;

        return new Response
        {
            Id = result.Value.Id,
            Status = result.Value.Status,
            UpdatedAt = result.Value.UpdatedAt
        };
    }
}
