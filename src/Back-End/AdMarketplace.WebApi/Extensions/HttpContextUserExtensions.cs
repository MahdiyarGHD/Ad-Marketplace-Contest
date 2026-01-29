using System.Security.Claims;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;

namespace AdMarketplace.Extensions;

public static class HttpContextUserExtensions
{
    public static async Task<ErrorOr<Database.Models.User>> GetCurrentUserAsync(this ClaimsPrincipal user, IUserService userService)
    {
        var userIdClaim = user.FindFirst("UserId")?.Value;
        if (string.IsNullOrWhiteSpace(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            return Error.Unauthorized("User.Unauthorized", "User not authenticated");

        return await userService.GetByUserIdAsync(userId);
    }
}

