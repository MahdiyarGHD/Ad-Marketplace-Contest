using AdMarketplace.Database;
using AdMarketplace.Domain.Types;
using AdMarketplace.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdMarketplace.Workers.DealLifecycle;

public class AutoPostingWorker(
    IServiceProvider serviceProvider,
    ILogger<AutoPostingWorker> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("AutoPostingWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessReadyDealsAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error in auto-posting cycle");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }

        logger.LogInformation("AutoPostingWorker stopped");
    }

    private async Task ProcessReadyDealsAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AdMarketDbContext>();
        var postingService = scope.ServiceProvider.GetRequiredService<IPostingService>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var channelVerificationService = scope.ServiceProvider.GetRequiredService<IChannelVerificationService>();

        var now = DateTimeOffset.UtcNow;

        var readyDeals = await dbContext.Deals
            .AsTracking()
            .Include(d => d.Channel)
                .ThenInclude(c => c.Owner)
            .Where(d => (d.Status == DealStatusType.DraftApproved || d.Status == DealStatusType.Scheduled) &&
                        d.DraftMessageId != null &&
                        (d.ScheduledPostTime == null || d.ScheduledPostTime <= now))
            .ToListAsync(ct);

        foreach (var deal in readyDeals)
        {
            try
            {
                await PostDealAsync(deal, postingService, notificationService, channelVerificationService, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to auto-post deal {DealId}", deal.Id);
            }
        }

        if (readyDeals.Count > 0)
            await dbContext.SaveChangesAsync(ct);
    }

    private async Task PostDealAsync(
        Database.Models.Deal deal,
        IPostingService postingService,
        INotificationService notificationService,
        IChannelVerificationService channelVerificationService,
        CancellationToken ct)
    {
        var verificationResult = await channelVerificationService.VerifyChannelReadinessAsync(deal.ChannelId, ct);
        if (verificationResult.IsError)
        {
            logger.LogWarning(
                "Channel verification failed for deal {DealId}: {Error}. Skipping auto-post.",
                deal.Id, verificationResult.FirstError.Description);
            return;
        }

        var channelChatId = deal.Channel.ChatId;
        var ownerTelegramId = deal.Channel.Owner.UserId;
        var draftMessageId = deal.DraftMessageId!.Value;

        logger.LogInformation(
            "Auto-posting deal {DealId} to channel {ChannelChatId} (draft msg {DraftMessageId})",
            deal.Id, channelChatId, draftMessageId);

        var result = await postingService.PostToChannelAsync(
            channelChatId, ownerTelegramId, draftMessageId, ct);

        if (result.IsError)
        {
            logger.LogError(
                "Auto-post failed for deal {DealId}: {Error}",
                deal.Id, result.FirstError.Description);
            return;
        }

        var postedMessageId = result.Value;

        string? textHash = null;
        if (deal.Channel.AgentId.HasValue && deal.Channel.Username is not null)
        {
            var hashResult = await postingService.GetMessageTextHashAsync(
                deal.Channel.AgentId.Value, deal.Channel.Username, postedMessageId, ct);

            if (!hashResult.IsError)
                textHash = hashResult.Value;
        }

        deal.MarkAsPosted(postedMessageId, textHash);

        await notificationService.NotifyDealPostedAsync(deal.Id, ct);

        logger.LogInformation(
            "Deal {DealId} posted successfully, message ID: {PostedMessageId}",
            deal.Id, postedMessageId);
    }
}
