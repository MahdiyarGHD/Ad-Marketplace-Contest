using AdMarketplace.Database;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdMarketplace.Workers.DealLifecycle;

public class PostVerificationWorker(
    IServiceProvider serviceProvider,
    ILogger<PostVerificationWorker> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan FallbackVerificationWindow = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("PostVerificationWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await VerifyPostedDealsAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error in post verification cycle");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }

        logger.LogInformation("PostVerificationWorker stopped");
    }

    private async Task VerifyPostedDealsAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AdMarketDbContext>();
        var postingService = scope.ServiceProvider.GetRequiredService<IPostingService>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var postedDeals = await dbContext.Deals
            .AsTracking()
            .Include(d => d.Channel)
            .Where(d => d.Status == DealStatusType.Verifying &&
                        d.PostedMessageId != null &&
                        d.Channel.AgentId != null &&
                        d.Channel.Username != null)
            .ToListAsync(ct);

        if (postedDeals.Count == 0)
            return;

        var now = DateTimeOffset.UtcNow;
        var modified = false;

        foreach (var deal in postedDeals)
        {
            try
            {
                var result = await postingService.VerifyPostAsync(
                    deal.Channel.AgentId!.Value,
                    deal.Channel.Username!,
                    deal.PostedMessageId!.Value,
                    deal.PostedTextHash,
                    ct);

                if (result.IsError)
                {
                    logger.LogWarning(
                        "Verification check failed for deal {DealId}: {Error}",
                        deal.Id, result.FirstError.Description);
                    continue;
                }

                var verification = result.Value;

                if (!verification.Exists)
                {
                    logger.LogWarning("Post deleted for deal {DealId}, disputing", deal.Id);
                    deal.UpdateStatus(DealStatusType.Disputed);
                    await notificationService.NotifyDealDisputedAsync(deal.Id, ct);
                    modified = true;
                    continue;
                }

                if (verification.WasEdited)
                {
                    logger.LogWarning("Post edited for deal {DealId}, disputing", deal.Id);
                    deal.UpdateStatus(DealStatusType.Disputed);
                    await notificationService.NotifyDealDisputedAsync(deal.Id, ct);
                    modified = true;
                    continue;
                }

                if (ShouldReleaseFunds(deal, verification.ViewCount, now))
                {
                    logger.LogInformation(
                        "Deal {DealId} passed verification (PriceType={PriceType}), releasing funds",
                        deal.Id, deal.PriceType);
                    deal.ReleaseFunds();
                    await notificationService.NotifyDealCompletedAsync(deal.Id, ct);
                    modified = true;

                    // Delete the post from the channel after verification passes
                    var deleteResult = await postingService.DeletePostAsync(
                        deal.Channel.ChatId,
                        deal.PostedMessageId!.Value,
                        ct);

                    if (deleteResult.IsError)
                    {
                        logger.LogWarning(
                            "Failed to delete post for deal {DealId}: {Error}",
                            deal.Id, deleteResult.FirstError.Description);
                    }
                    else
                    {
                        deal.MarkPostAsDeleted();
                        await notificationService.NotifyPostDeletedAsync(deal.Id, ct);
                        logger.LogInformation(
                            "Deleted expired post for deal {DealId}, both parties notified",
                            deal.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error verifying deal {DealId}", deal.Id);
            }
        }

        if (modified)
            await dbContext.SaveChangesAsync(ct);
    }

    private static bool ShouldReleaseFunds(Database.Models.Deal deal, int currentViewCount, DateTimeOffset now)
    {
        if (!deal.ActualPostTime.HasValue)
            return false;

        if (deal.PriceType == PriceType.PerThousandViews)
        {
            if (!deal.RequiredViewCount.HasValue)
                return false;

            return currentViewCount >= deal.RequiredViewCount.Value;
        }

        if (deal.PostVerifyAt.HasValue)
            return now >= deal.PostVerifyAt.Value;

        return now - deal.ActualPostTime.Value >= FallbackVerificationWindow;
    }
}
