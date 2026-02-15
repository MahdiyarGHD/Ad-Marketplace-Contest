using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Contracts.Common;
using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface IUserService
{
    Task<ErrorOr<User>> EnsureExistsAsync(InitDataUserContract initData);
    Task<ErrorOr<User>> GetByUserIdAsync(long userId);
    Task<ErrorOr<UserTransaction>> GetUserTransactions(Guid id);
}