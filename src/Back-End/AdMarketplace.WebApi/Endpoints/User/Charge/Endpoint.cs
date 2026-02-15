using AdMarketplace.Database.Models;
using AdMarketplace.Endpoints.User.Me;
using AdMarketplace.Extensions;
using AdMarketplace.Infra.Interfaces;
using AdMarketplace.Infra.Services.PaymentServices;
using ErrorOr;
using FastEndpoints;

namespace AdMarketplace.Endpoints.User.Charge;

public class Endpoint(IUserService userService, ITransactionService transactionService, TonPaymentService tonPaymentService)
    : Endpoint<Request, ErrorOr<Success>>
{
    public override void Configure()
    {
        Get("/api/user/charge");
    }

    public override async Task<ErrorOr<Success>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var userResult = await User.GetCurrentUserAsync(userService);
        if (userResult.IsError)
            return userResult.Errors;
        var user = userResult.Value;

        var isExists = await transactionService.TransactionExistsAsync(req.TransactionHash);
        if(isExists)
            return Error.Conflict("Transaction.AlreadyExists", "The specified transaction is already used.");
        
        var amountTon = await tonPaymentService.GetDepositAmountAsync(
            req.TransactionHash,
            expectedMemo: user.Id.ToString(),
            expectedSender: req.WalletAddress 
        );

        if (amountTon == null)
            return Error.Validation("Transaction.Invalid", "Could not verify transaction on the blockchain. Check hash, sender, or memo.");

        var checkNetwork = await transactionService.RecordPaymentAsync(new UserTransaction
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TransactionHash = req.TransactionHash,
            Amount = amountTon.Value,
            LogicalTime = 0, 
            Destination = "BusinessWallet", 
            CreatedAt = DateTime.UtcNow
        });
        
        if (checkNetwork.IsError)
            return checkNetwork.Errors;
        
        return Result.Success;
    }
}
