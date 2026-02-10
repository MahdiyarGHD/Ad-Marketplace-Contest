using System.Threading.RateLimiting;
using AdMarketplace.Database;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdMarketplace.Workers.ChannelAnalytics;

public class AnalyticsConsumerWorker(
    IAnalyticsUpdateQueue queue,
    IServiceProvider serviceProvider,
    ILogger<AnalyticsConsumerWorker> logger) : BackgroundService
{
    private readonly RateLimiter _rateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
    {
        TokenLimit = 20,
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 1000,
        ReplenishmentPeriod = TimeSpan.FromSeconds(5),
        TokensPerPeriod = 1,
        AutoReplenishment = true
    });

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Analytics Update Background Service started");

        await foreach (var message in queue.DequeueAllAsync(stoppingToken))
        {
            using var lease = await _rateLimiter.AcquireAsync(permitCount: 1, stoppingToken);

            if (!lease.IsAcquired)
            {
                logger.LogWarning("Failed to acquire rate limit lease for channel {ChannelId}", message.ChannelId);
                continue;
            }

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AdMarketDbContext>();

            var channel = await dbContext.Channels
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == message.ChannelId, stoppingToken);

            if (channel is null || channel.Status != ChannelStatusType.Ready)
            {
                logger.LogInformation("Skipping analytics update for channel {ChannelId} — not ready", message.ChannelId);
                continue;
            }

            var analyticsUpdateService = scope.ServiceProvider.GetRequiredService<IAnalyticsUpdateService>();

            await analyticsUpdateService.UpdateChannelAnalyticsAsync(
                message.ChannelId,
                stoppingToken);
        }

        logger.LogInformation("Analytics Update Background Service stopped");
    }

    public override void Dispose()
    {
        _rateLimiter.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
