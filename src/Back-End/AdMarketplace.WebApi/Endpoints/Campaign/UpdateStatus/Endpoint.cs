using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Campaign.UpdateStatus;

public class Endpoint(ICampaignService campaignService, IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Patch("/api/campaigns/{Id}/status");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await campaignService.UpdateStatusAsync(req.Id, userResult.Value.Id, req.Status);

        if (result.IsError)
            return result.Errors;

        var campaign = result.Value;

        return new Response
        {
            Id = campaign.Id,
            Status = campaign.Status,
            UpdatedAt = campaign.UpdatedAt
        };
    }
}
