using AdMarketplace.Database.Models;
using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface ITransactionService
{
    Task<ErrorOr<Success>> RecordPaymentAsync(UserTransaction tx);
    Task<bool> TransactionExistsAsync(string transactionHash);
}