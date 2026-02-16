using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.ChannelApplication.GetByChannel;

public class Endpoint(
    IChannelApplicationService channelApplicationService,
    IChannelService channelService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/channels/{ChannelId}/applications");
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

        var result = await channelApplicationService.GetByChannelIdAsync(req.ChannelId, req.Skip, req.Take);

        if (result.IsError)
            return result.Errors;

        var applications = result.Value.Select(a => new ChannelApplicationItem
        {
            Id = a.Id,
            AdvertiserId = a.AdvertiserId,
            AdvertiserName = a.Advertiser.FirstName,
            ProposedAdFormat = a.ProposedAdFormat,
            ProposedPriceType = a.ProposedPriceType,
            ProposedPriceTon = a.ProposedPriceTon,
            ProposedPostingTime = a.ProposedPostingTime,
            Message = a.Message,
            Status = a.Status,
            RejectionReason = a.RejectionReason,
            CounterAdFormat = a.CounterAdFormat,
            CounterPriceType = a.CounterPriceType,
            CounterPriceTon = a.CounterPriceTon,
            CounterPostingTime = a.CounterPostingTime,
            CounterMessage = a.CounterMessage,
            LastCounterByUserId = a.LastCounterByUserId,
            CreatedAt = a.CreatedAt
        }).ToList();

        return new Response { Applications = applications };
    }
}
