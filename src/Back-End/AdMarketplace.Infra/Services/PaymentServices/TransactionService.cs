using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services.PaymentServices;

public class TransactionService(AdMarketDbContext dbContext) : ITransactionService
{
    /// <summary>
    /// Checks if a transaction has already been recorded in our system.
    /// </summary>
    public async Task<bool> TransactionExistsAsync(string transactionHash)
    {
        var normalizedHash = transactionHash.ToLower().Trim();
        return await dbContext.UserTransactions
            .AnyAsync(t => t.TransactionHash == normalizedHash);
    }

    /// <summary>
    /// Records the transaction and updates the user's balance in one atomic operation.
    /// </summary>
    public async Task<ErrorOr<Success>> RecordPaymentAsync(UserTransaction tx)
    {
        if (await TransactionExistsAsync(tx.TransactionHash))
            return Error.Conflict("Transaction.Duplicate", "This transaction has already been processed.");

        using var dbTransaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == tx.UserId);

            if (user == null)
                return Error.NotFound("User.NotFound", "User not found.");

            user.Balance += tx.Amount;

            tx.TransactionHash = tx.TransactionHash.ToLower().Trim();
            dbContext.UserTransactions.Add(tx);

            await dbContext.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return Result.Success;
        }
        catch (Exception)
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }
}