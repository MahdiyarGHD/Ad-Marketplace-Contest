using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Deal.GetByChannel;

public class Endpoint(
    IDealService dealService,
    IUserService userService,
    IChannelService channelService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/deals/channel/{ChannelId}");
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

        var result = await dealService.GetByChannelIdAsync(req.ChannelId, req.Skip, req.Take);
        if (result.IsError)
            return result.Errors;

        var deals = result.Value.Select(d => new DealItem
        {
            Id = d.Id,
            CampaignId = d.CampaignId,
            CampaignTitle = d.Campaign?.Title,
            ChannelId = d.ChannelId,
            ChannelTitle = d.Channel.Title,
            AmountTon = d.AmountTon,
            Status = d.Status,
            DraftStatus = d.DraftStatus,
            ScheduledPostTime = d.ScheduledPostTime,
            CreatedAt = d.CreatedAt
        }).ToList();

        return new Response { Deals = deals };
    }
}
