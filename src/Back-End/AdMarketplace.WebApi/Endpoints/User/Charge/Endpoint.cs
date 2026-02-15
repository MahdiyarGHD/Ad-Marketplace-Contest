using AdMarketplace.Endpoints.User.Me;
using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.User.Charge;

public class Endpoint(IUserService userService, ITransactionService transactionService)
    : Endpoint<Request, ErrorOr<Success>>
{
    public override void Configure()
    {
        Get("/api/user/charge");
    }

    public async override Task<ErrorOr<Success>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;
        var user = userResult.Value;

        // var isExists = 
        
        return Result.Success;
    }
}
