using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using AdMarketplace.Infra.Services.PaymentServices;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.User.Me;

public class Endpoint(IUserService userService, TonPaymentService tonPaymentService)
    : EndpointWithoutRequest<ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/user/me");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(CancellationToken ct)
    {
        var result = await tonPaymentService.RefundAsync("0QBgbSx49lwLITzlB_Hla3OaTwSonMLaDaBmyyqsvHMPih83", 0.35M, "Ehskh");
        
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var user = userResult.Value;

        return new Response
        {
            Id = user.Id,
            Balance = user.Balance,
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt
        };
    }
}
