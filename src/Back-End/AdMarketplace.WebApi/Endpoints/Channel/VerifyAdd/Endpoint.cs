using AdMarketplace.Extensions;
using AdMarketplace.Infra;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Channel.VerifyAdd;

public class Endpoint(IUserChannelConnectionService connectionService, IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/channels/verify-add");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var lookBackSeconds = Math.Clamp(req.LookBackSeconds, 60, 600);
        var sinceUtc = DateTimeOffset.UtcNow.AddSeconds(-lookBackSeconds);

        var linksResult = await connectionService.GetRecentByUserIdAsync(userResult.Value.Id, sinceUtc);
        if (linksResult.IsError)
            return linksResult.Errors;

        var chatIds = linksResult.Value.Select(x => x.ChatId).Distinct().ToList();

        return new Response
        {
            HasNewChannel = chatIds.Count > 0,
            ChatIds = chatIds
        };
    }
}

