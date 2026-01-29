using AdMarketplace.Extensions;
using AdMarketplace.Infra;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Channel.VerifyAdd;

public class Endpoint(IUserChannelConnectionService connectionService, IUserService userService)
    : Endpoint<Request, ErrorOr<List<Response>>>
{
    public override void Configure()
    {
        Post("/api/channels/verify-add");
    }

    public override async Task<ErrorOr<List<Response>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var lookBackSeconds = Math.Clamp(req.LookBackSeconds, 60, 60 * 15);
        var sinceUtc = DateTimeOffset.UtcNow.AddSeconds(-lookBackSeconds);

        var linksResult = await connectionService.GetRecentByUserIdAsync(userResult.Value.Id, sinceUtc);
        if (linksResult.IsError)
            return linksResult.Errors;

        var results = linksResult.Value.Select(x => new {x.ChatId, x.Title}).Distinct();

        return results.Select(res => new Response
        {
            Title = res.Title,
            ChatId = res.ChatId
        }).ToList();
    }
}

