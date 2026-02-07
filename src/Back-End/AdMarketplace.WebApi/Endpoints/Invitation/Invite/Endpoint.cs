using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Invitation.Invite;

public class Endpoint(
    ICampaignInvitationService invitationService,
    IUserService userService,
    ICampaignService campaignService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/invitations");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var campaignResult = await campaignService.GetByIdAsync(req.CampaignId);
        if (campaignResult.IsError)
            return campaignResult.Errors;

        if (campaignResult.Value.AdvertiserId != userResult.Value.Id)
            return Error.Forbidden("Campaign.NotOwner", "You are not the owner of this campaign");

        var result = await invitationService.CreateAsync(
            campaignId: req.CampaignId,
            channelId: req.ChannelId,
            proposedAdFormat: req.ProposedAdFormat,
            proposedPriceType: req.ProposedPriceType,
            proposedPriceTon: req.ProposedPriceTon,
            proposedPostingTime: req.ProposedPostingTime,
            message: req.Message);

        if (result.IsError)
            return result.Errors;

        var invitation = result.Value;

        return new Response
        {
            Id = invitation.Id,
            CampaignId = invitation.CampaignId,
            ChannelId = invitation.ChannelId,
            ProposedAdFormat = invitation.ProposedAdFormat,
            ProposedPriceType = invitation.ProposedPriceType,
            ProposedPriceTon = invitation.ProposedPriceTon,
            Status = invitation.Status,
            CreatedAt = invitation.CreatedAt
        };
    }
}

