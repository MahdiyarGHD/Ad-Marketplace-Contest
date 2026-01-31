using AdMarketplace.Domain.Contracts.Responses;
using AdMarketplace.Extensions;
using AdMarketplace.Infra;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;
using Mapster;

namespace AdMarketplace.Endpoints.Channel.MyChannels;

public class Endpoint(IChannelService channelService, IUserService userService)
    : EndpointWithoutRequest<ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/channels/my");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await channelService.GetByOwnerIdAsync(userResult.Value.Id);

        if (result.IsError)
            return result.Errors;

        return new Response
        {
            Channels = result.Value.Adapt<List<ChannelItem>>()
        };
    }
}
