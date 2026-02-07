using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Deal.CreateFromInvitation;

public class Endpoint(
    IDealService dealService,
    ICampaignInvitationService invitationService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/deals/from-invitation");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var invitationResult = await invitationService.GetByIdAsync(req.InvitationId);
        if (invitationResult.IsError)
            return invitationResult.Errors;

        var invitation = invitationResult.Value;

        var result = await dealService.CreateAsync(
            campaignId: req.CampaignId,
            applicationId: null,
            invitationId: req.InvitationId,
            channelApplicationId: null,
            channelId: req.ChannelId,
            advertiserId: userResult.Value.Id,
            amountTon: invitation.ProposedPriceTon,
            adFormat: invitation.ProposedAdFormat,
            priceType: invitation.ProposedPriceType,
            scheduledPostTime: invitation.ProposedPostingTime,
            escrowWalletAddress: req.EscrowWalletAddress);

        if (result.IsError)
            return result.Errors;

        var deal = result.Value;

        return new Response
        {
            Id = deal.Id,
            CampaignId = deal.CampaignId,
            InvitationId = deal.InvitationId!.Value,
            AdFormat = deal.AdFormat,
            PriceType = deal.PriceType,
            AmountTon = deal.AmountTon,
            Status = deal.Status,
            EscrowWalletAddress = deal.EscrowWalletAddress,
            CreatedAt = deal.CreatedAt
        };
    }
}
