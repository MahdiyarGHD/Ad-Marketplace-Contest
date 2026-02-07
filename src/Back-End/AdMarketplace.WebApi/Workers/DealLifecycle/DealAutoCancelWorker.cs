using AdMarketplace.Database;
using AdMarketplace.Domain.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdMarketplace.Workers.DealLifecycle;

public class DealAutoCancelWorker(
    IServiceProvider serviceProvider,
    ILogger<DealAutoCancelWorker> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("DealAutoCancelWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredDealsAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error processing expired deals");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }

        logger.LogInformation("DealAutoCancelWorker stopped");
    }

    private async Task ProcessExpiredDealsAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AdMarketDbContext>();

        var now = DateTimeOffset.UtcNow;

        var expiredDeals = await dbContext.Deals
            .AsTracking()
            .Where(d => d.AutoCancelAt.HasValue &&
                        d.AutoCancelAt.Value <= now &&
                        d.Status != DealStatusType.Completed &&
                        d.Status != DealStatusType.Cancelled &&
                        d.Status != DealStatusType.Refunded)
            .ToListAsync(ct);

        if (expiredDeals.Count == 0)
            return;

        logger.LogInformation("Found {Count} expired deals to process", expiredDeals.Count);

        foreach (var deal in expiredDeals)
        {
            if (deal.HasFundedEscrow)
            {
                deal.Refund();
                logger.LogInformation("Refunded expired deal {DealId} (was {Status})", deal.Id, deal.Status);
            }
            else
            {
                deal.Cancel();
                logger.LogInformation("Cancelled expired deal {DealId} (was {Status})", deal.Id, deal.Status);
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
