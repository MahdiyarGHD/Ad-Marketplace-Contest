using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.ChannelApplication.MyApplications;

public class Endpoint(
    IChannelApplicationService channelApplicationService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/my-channel-applications");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await channelApplicationService.GetByAdvertiserIdAsync(
            userResult.Value.Id, req.Skip, req.Take);

        if (result.IsError)
            return result.Errors;

        var applications = result.Value.Select(a => new MyChannelApplicationItem
        {
            Id = a.Id,
            ChannelId = a.ChannelId,
            ChannelTitle = a.Channel.Title,
            ChannelUsername = a.Channel.Username,
            ProposedAdFormat = a.ProposedAdFormat,
            ProposedPriceType = a.ProposedPriceType,
            ProposedPriceTon = a.ProposedPriceTon,
            ProposedPostingTime = a.ProposedPostingTime,
            Message = a.Message,
            Status = a.Status,
            RejectionReason = a.RejectionReason,
            CreatedAt = a.CreatedAt
        }).ToList();

        return new Response { Applications = applications };
    }
}
