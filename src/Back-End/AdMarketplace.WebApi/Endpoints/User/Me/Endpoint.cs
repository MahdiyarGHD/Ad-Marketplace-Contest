using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.User.Me;

public class Endpoint(IUserService userService)
    : EndpointWithoutRequest<ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/users/me");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var user = userResult.Value;

        return new Response
        {
            Id = user.Id,
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt
        };
    }
}
