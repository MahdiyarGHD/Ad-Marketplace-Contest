using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Infra.Helpers;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Authentication.Authenticate;

public class Endpoint(InitDataHelper initDataHelper, ILogger<Endpoint> logger)
    : Endpoint<Request, ErrorOr<InitDataUserContract>>
{
    public override void Configure()
    {
        Post("/api/authentication/authenticate");
        AllowAnonymous();
    }
    public override async Task<ErrorOr<InitDataUserContract>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var validationResult = await initDataHelper.ValidateInitDataAsync(req.InitData);
        return validationResult;
    }
}