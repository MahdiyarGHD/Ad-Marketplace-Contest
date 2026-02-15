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

        var userId = userResult.Value.Id;
        var result = await dealService.GetByUserIdAsync(userId, req.Skip, req.Take);

        if (result.IsError)
            return result.Errors;

        var deals = result.Value.Select(d => new DealItem
        {
            Id = d.Id,
            CampaignId = d.CampaignId,
            CampaignTitle = d.Campaign?.Title,
            ChannelId = d.ChannelId,
            ChannelTitle = d.Channel.Title,
            AdvertiserId = d.AdvertiserId,
            AdvertiserFirstName = d.Advertiser.FirstName,
            AdvertiserUserName = d.Advertiser.UserName,
            Role = d.AdvertiserId == userId ? DealRoleType.Advertiser : DealRoleType.ChannelOwner,
            AmountTon = d.AmountTon,
            Status = d.Status,
            DraftStatus = d.DraftStatus,
            ScheduledPostTime = d.ScheduledPostTime,
            CreatedAt = d.CreatedAt
        }).ToList();

        return new Response { Deals = deals };
    }
}
