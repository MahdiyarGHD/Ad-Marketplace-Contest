using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Deal.CreateFromChannelApplication;

public class Endpoint(
    IDealService dealService,
    IChannelApplicationService channelApplicationService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/deals/from-channel-application");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var applicationResult = await channelApplicationService.GetByIdAsync(req.ChannelApplicationId);
        if (applicationResult.IsError)
            return applicationResult.Errors;

        var application = applicationResult.Value;

        var result = await dealService.CreateAsync(
            campaignId: null,
            applicationId: null,
            invitationId: null,
            channelApplicationId: req.ChannelApplicationId,
            channelId: req.ChannelId,
            advertiserId: userResult.Value.Id,
            amountTon: application.ProposedPriceTon,
            adFormat: application.ProposedAdFormat,
            priceType: application.ProposedPriceType,
            scheduledPostTime: application.ProposedPostingTime,
            escrowWalletAddress: req.EscrowWalletAddress);

        if (result.IsError)
            return result.Errors;

        var deal = result.Value;

        return new Response
        {
            Id = deal.Id,
            ChannelApplicationId = deal.ChannelApplicationId!.Value,
            AdFormat = deal.AdFormat,
            PriceType = deal.PriceType,
            AmountTon = deal.AmountTon,
            Status = deal.Status,
            EscrowWalletAddress = deal.EscrowWalletAddress,
            CreatedAt = deal.CreatedAt
        };
    }
}
