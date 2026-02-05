using AdMarketplace.Database.Models;
using ErrorOr;

namespace AdMarketplace.Infra.Interfaces;

public interface IAgentService
{
    Task<ErrorOr<Channel>> AttachAgentToChannel(Guid channelId);
}
