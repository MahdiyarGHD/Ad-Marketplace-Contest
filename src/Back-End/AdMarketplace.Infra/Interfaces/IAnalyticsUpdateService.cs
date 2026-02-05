using AdMarketplace.Infra.Interfaces;

namespace AdMarketplace.Infra.Interfaces;

public interface IAnalyticsUpdateService
{
    Task UpdateChannelAnalyticsAsync(Guid channelId, CancellationToken cancellationToken = default);
}
