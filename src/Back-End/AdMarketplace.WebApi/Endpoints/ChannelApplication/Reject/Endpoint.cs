using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.ChannelApplication.Reject;

public class Endpoint(
    IChannelApplicationService channelApplicationService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/channel-applications/{Id}/reject");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await channelApplicationService.RejectAsync(req.Id, userResult.Value.Id, req.Reason);

        if (result.IsError)
            return result.Errors;

        var application = result.Value;

        return new Response
        {
            Id = application.Id,
            Status = application.Status,
            RejectionReason = application.RejectionReason,
            UpdatedAt = application.UpdatedAt
        };
    }
}
