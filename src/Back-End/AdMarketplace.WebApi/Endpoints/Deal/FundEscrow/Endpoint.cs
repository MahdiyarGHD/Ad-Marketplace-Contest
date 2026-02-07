using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.Deal.FundEscrow;

public class Endpoint(
    IDealService dealService,
    IUserService userService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Post("/api/deals/{Id}/fund");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;

        var result = await dealService.FundEscrowAsync(req.Id, userResult.Value.Id, req.TransactionHash, req.WalletAddress);

        if (result.IsError)
            return result.Errors;

        var deal = result.Value;

        return new Response
        {
            Id = deal.Id,
            Status = deal.Status,
            TransactionHash = deal.TransactionHash!,
            EscrowWalletAddress = deal.EscrowWalletAddress!,
            UpdatedAt = deal.UpdatedAt
        };
    }
}
