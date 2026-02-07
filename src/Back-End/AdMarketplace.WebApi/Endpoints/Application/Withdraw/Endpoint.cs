using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Application.Withdraw;

public class Endpoint(
    ICampaignApplicationService applicationService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Delete("/api/applications/{Id}");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await applicationService.WithdrawAsync(req.Id, userResult.Value.Id);

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
