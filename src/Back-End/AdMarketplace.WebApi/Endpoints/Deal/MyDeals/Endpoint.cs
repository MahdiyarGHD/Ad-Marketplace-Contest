using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Deal.MyDeals;

public class Endpoint(
    IDealService dealService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/deals/my");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await dealService.GetByAdvertiserIdAsync(userResult.Value.Id, req.Skip, req.Take);

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
