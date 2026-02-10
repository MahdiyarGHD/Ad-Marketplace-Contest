using AdMarketplace.Database;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdMarketplace.Workers.ChannelAnalytics;

public class AnalyticsProducerWorker(
    IAnalyticsUpdateQueue queue,
    IServiceProvider serviceProvider,
    ILogger<AnalyticsProducerWorker> logger) : BackgroundService
{
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(6);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("AnalyticsProducerWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await EnqueueChannelsForUpdateAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error in analytics producer cycle");
            }

            await Task.Delay(RefreshInterval, stoppingToken);
        }

        logger.LogInformation("AnalyticsProducerWorker stopped");
    }

    private async Task EnqueueChannelsForUpdateAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AdMarketDbContext>();

        var channelIds = await dbContext.Channels
            .Where(c => c.Status == ChannelStatusType.Ready)
            .Select(c => c.Id)
            .ToListAsync(ct);

        if (channelIds.Count == 0)
            return;

        var now = DateTimeOffset.UtcNow;
        var enqueued = 0;

        foreach (var channelId in channelIds)
        {
            await queue.EnqueueAsync(new AnalyticsUpdateMessage(channelId, now), ct);
            enqueued++;
        }

        logger.LogInformation("Enqueued {Count} channels for analytics update", enqueued);
    }
}
