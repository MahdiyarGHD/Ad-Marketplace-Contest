using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Invitation.GetByCampaign;

public class Endpoint(
    ICampaignInvitationService invitationService,
    IUserService userService,
    ICampaignService campaignService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/campaigns/{CampaignId}/invitations");
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

        var result = await invitationService.GetByCampaignIdAsync(req.CampaignId, req.Skip, req.Take);
        if (result.IsError)
            return result.Errors;

        var invitations = result.Value.Select(i => new InvitationItem
        {
            Id = i.Id,
            CampaignId = i.CampaignId,
            ChannelId = i.ChannelId,
            ChannelTitle = i.Channel.Title,
            ProposedAdFormat = i.ProposedAdFormat,
            ProposedPriceType = i.ProposedPriceType,
            ProposedPriceTon = i.ProposedPriceTon,
            Status = i.Status,
            CreatedAt = i.CreatedAt
        }).ToList();

        return new Response { Invitations = invitations };
    }
}
