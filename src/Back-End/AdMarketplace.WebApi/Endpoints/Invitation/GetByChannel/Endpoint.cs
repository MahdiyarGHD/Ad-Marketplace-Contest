using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Invitation.GetByChannel;

public class Endpoint(
    ICampaignInvitationService invitationService,
    IUserService userService,
    IChannelService channelService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/channels/{ChannelId}/invitations");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var channelResult = await channelService.GetByIdAsync(req.ChannelId);
        if (channelResult.IsError)
            return channelResult.Errors;

        if (channelResult.Value.OwnerId != userResult.Value.Id)
            return Error.Forbidden("Channel.NotOwner", "You are not the owner of this channel");

        var result = await invitationService.GetByChannelIdAsync(req.ChannelId, req.Skip, req.Take);
        if (result.IsError)
            return result.Errors;

        var invitations = result.Value.Select(i => new InvitationItem
        {
            Id = i.Id,
            CampaignId = i.CampaignId,
            CampaignTitle = i.Campaign.Title,
            ChannelId = i.ChannelId,
            ProposedAdFormat = i.ProposedAdFormat,
            ProposedPriceType = i.ProposedPriceType,
            ProposedPriceTon = i.ProposedPriceTon,
            ProposedPostingTime = i.ProposedPostingTime,
            Message = i.Message,
            Status = i.Status,
            CreatedAt = i.CreatedAt
        }).ToList();

        return new Response { Invitations = invitations };
    }
}
