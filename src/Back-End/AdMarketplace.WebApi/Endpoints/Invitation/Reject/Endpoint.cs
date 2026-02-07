using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Invitation.Reject;

public class Endpoint(
    ICampaignInvitationService invitationService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/invitations/{Id}/reject");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await invitationService.RejectAsync(req.Id, userResult.Value.Id, req.Feedback);

        if (result.IsError)
            return result.Errors;

        var invitation = result.Value;

        return new Response
        {
            Id = invitation.Id,
            Status = invitation.Status,
            RejectionReason = invitation.RejectionReason,
            UpdatedAt = invitation.UpdatedAt
        };
    }
}
