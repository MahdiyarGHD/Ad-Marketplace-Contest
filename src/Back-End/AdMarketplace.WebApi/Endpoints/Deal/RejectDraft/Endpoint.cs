using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Deal.RejectDraft;

public class Endpoint(
    IDealService dealService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/deals/{Id}/reject");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await dealService.RejectDraftAsync(req.Id, userResult.Value.Id, req.Feedback);

        if (result.IsError)
            return result.Errors;

        var deal = result.Value;

        return new Response
        {
            Id = deal.Id,
            Status = deal.Status,
            DraftStatus = deal.DraftStatus,
            AdvertiserFeedback = deal.AdvertiserFeedback,
            UpdatedAt = deal.UpdatedAt
        };
    }
}
