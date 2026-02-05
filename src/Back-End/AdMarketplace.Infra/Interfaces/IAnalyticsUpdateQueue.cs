using AdMarketplace.Domain.Contracts.Common;

namespace AdMarketplace.Infra.Interfaces;

public interface IAnalyticsUpdateQueue
{
    ValueTask EnqueueAsync(AnalyticsUpdateMessage message, CancellationToken cancellationToken = default);
    
    IAsyncEnumerable<AnalyticsUpdateMessage> DequeueAllAsync(CancellationToken cancellationToken = default);
    
    int GetQueueCount();
}
