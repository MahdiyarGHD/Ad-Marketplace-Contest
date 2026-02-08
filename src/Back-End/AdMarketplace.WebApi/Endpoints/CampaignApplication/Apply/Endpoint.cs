using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.CampaignApplication.Apply;

public class Endpoint(
    ICampaignApplicationService campaignApplicationService,
    IUserService userService,
    IChannelService channelService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/campaign-applications");
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

        var result = await campaignApplicationService.CreateAsync(
            campaignId: req.CampaignId,
            channelId: req.ChannelId,
            proposedAdFormat: req.ProposedAdFormat,
            proposedPriceType: req.ProposedPriceType,
            proposedPriceTon: req.ProposedPriceTon,
            proposedPostingTime: req.ProposedPostingTime,
            message: req.Message);

        if (result.IsError)
            return result.Errors;

        var application = result.Value;

        return new Response
        {
            Id = application.Id,
            CampaignId = application.CampaignId,
            ChannelId = application.ChannelId,
            ProposedAdFormat = application.ProposedAdFormat,
            ProposedPriceType = application.ProposedPriceType,
            ProposedPriceTon = application.ProposedPriceTon,
            Status = application.Status,
            CreatedAt = application.CreatedAt
        };
    }
}
