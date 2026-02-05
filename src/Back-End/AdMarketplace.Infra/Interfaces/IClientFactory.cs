using ErrorOr;
using WTelegram;

namespace AdMarketplace.Infra.Interfaces;

public interface IClientFactory
{
    Task<ErrorOr<Client>> CreateClientAsync(Guid agentId);
}
