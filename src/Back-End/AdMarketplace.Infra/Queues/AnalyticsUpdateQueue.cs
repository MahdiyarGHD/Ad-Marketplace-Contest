using System.Runtime.CompilerServices;
using System.Threading.Channels;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Infra.Interfaces;

namespace AdMarketplace.Infra.Queues;

public class AnalyticsUpdateQueue : IAnalyticsUpdateQueue
{
    private readonly Channel<AnalyticsUpdateMessage> _channel;

    public AnalyticsUpdateQueue()
    {
        var options = new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
        };
        
        _channel = Channel.CreateBounded<AnalyticsUpdateMessage>(options);
    }

    public async ValueTask EnqueueAsync(AnalyticsUpdateMessage message, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(message, cancellationToken);
    }

    public async IAsyncEnumerable<AnalyticsUpdateMessage> DequeueAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return message;
        }
    }

    public int GetQueueCount()
    {
        return _channel.Reader.Count;
    }
}
