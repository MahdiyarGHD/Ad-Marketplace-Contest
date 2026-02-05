using AdMarketplace.Database;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;
using Telegram.Bot;
using TL;
using JsonArray = System.Text.Json.Nodes.JsonArray;
using JsonObject = System.Text.Json.Nodes.JsonObject;

namespace AdMarketplace.Infra.Services.ChannelServices;

public class AnalyticsUpdateService(
    AdMarketDbContext dbContext,
    ITelegramBotClient botClient,
    IClientFactory clientFactory,
    ILogger<AnalyticsUpdateService> logger) : IAnalyticsUpdateService
{
    public async Task UpdateChannelAnalyticsAsync(
        Guid channelId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var channel = await dbContext.Channels
                .Include(c => c.Agent)
                .AsTracking()
                .FirstOrDefaultAsync(c => c.Id == channelId, cancellationToken);

            if (channel is null)
            {
                logger.LogWarning("Channel {ChannelId} not found for analytics update", channelId);
                return;
            }

            var memberCount = await botClient.GetChatMemberCount(channel.ChatId, cancellationToken);
            channel.UpdateSubscriberCount(memberCount);

            if (channel.AgentId.HasValue)
            {
                await UpdateDetailedAnalyticsAsync(channel, cancellationToken);
            }
            else
            {
                logger.LogInformation(
                    "Channel {ChannelId} has no agent attached, skipping detailed analytics",
                    channelId);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            
            logger.LogInformation(
                "Updated analytics for channel {ChannelId} (ChatId: {ChatId}) - Members: {MemberCount}",
                channelId,
                channel.ChatId,
                memberCount);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to update analytics for channel {ChannelId}",
                channelId);
        }
    }

    private async Task UpdateDetailedAnalyticsAsync(
        Database.Models.Channel channel,
        CancellationToken cancellationToken)
    {
        WTelegram.Client? client = null;
        
        try
        {
            client = await CreateAndLoginClientAsync(channel.AgentId!.Value);
            if (client is null) return;

            var tlChannel = await ResolveTelegramChannelAsync(client, channel);
            if (tlChannel is null) return;

            var channelFull = await GetFullChannelInfoAsync(client, tlChannel, channel.Id);
            if (channelFull is null) return;

            var stats = await GetChannelStatsAsync(client, tlChannel);
            var (averageViews, languages) = await ParseChannelStatsAsync(client, tlChannel, stats);
            var premiums = await GetPremiumPercentageAsync(client, tlChannel);

            UpdateChannelWithAnalytics(channel, channelFull.participants_count, averageViews, premiums, languages);

            logger.LogInformation(
                "Updated analytics for channel {ChannelId} - Subscribers: {Subscribers}, AvgViews: {AvgViews}, Languages: {LangCount}",
                channel.Id, channelFull.participants_count, averageViews, languages.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update detailed analytics for channel {ChannelId}", channel.Id);
        }
        finally
        {
            client?.Dispose();
        }
    }

    private async Task<WTelegram.Client?> CreateAndLoginClientAsync(Guid agentId)
    {
        var clientResult = await clientFactory.CreateClientAsync(agentId);
        if (clientResult.IsError)
        {
            logger.LogWarning("Failed to create WTelegram client: {Errors}",
                string.Join(", ", clientResult.Errors.Select(e => e.Description)));
            return null;
        }

        var loginResult = await clientResult.Value.LoginUserIfNeeded();
        if (loginResult is null)
        {
            logger.LogWarning("Failed to login agent {AgentId}", agentId);
            return null;
        }

        return clientResult.Value;
    }

    private async Task<TL.Channel?> ResolveTelegramChannelAsync(WTelegram.Client client, Database.Models.Channel channel)
    {
        var channelUsername = channel.Username ?? channel.ChatId.ToString();
        var resolvedPeer = await client.Contacts_ResolveUsername(channelUsername);
        var tlChannel = resolvedPeer?.chats?.Values.FirstOrDefault() as TL.Channel;

        if (tlChannel is null)
        {
            logger.LogWarning("Could not resolve Telegram channel for {ChannelId}", channel.Id);
        }

        return tlChannel;
    }

    private async Task<ChannelFull?> GetFullChannelInfoAsync(WTelegram.Client client, TL.Channel tlChannel, Guid channelId)
    {
        var fullChannel = await client.Channels_GetFullChannel(tlChannel);
        var channelFull = fullChannel?.full_chat as ChannelFull;

        if (channelFull is null)
        {
            logger.LogWarning("Could not get full channel info for {ChannelId}", channelId);
        }

        return channelFull;
    }

    private async Task<Stats_BroadcastStats?> GetChannelStatsAsync(WTelegram.Client client, TL.Channel channel)
    {
        try
        {
            return await client.Stats_GetBroadcastStats(channel);
        }
        catch (RpcException)
        {
            return null; // User is not an admin or stats are not available
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch broadcast stats for channel {ChannelId}", channel.ID);
            return null;
        }
    }

    private async Task<(int averageViews, List<LanguageDistributionContract> languages)> ParseChannelStatsAsync(
        WTelegram.Client client,
        TL.Channel tlChannel,
        Stats_BroadcastStats? stats)
    {
        if (stats is not null)
        {
            var averageViews = (int)stats.views_per_post.current;
            var languages = await ParseLanguageDistributionAsync(client, stats);
            
            logger.LogInformation("Using official stats - AvgViews: {AvgViews}, Languages: {LangCount}",
                averageViews, languages.Count);
            
            return (averageViews, languages);
        }

        var fallbackViews = await CalculateAverageViewsFromMessagesAsync(client, tlChannel);
        logger.LogInformation("Using fallback analytics - AvgViews: {AvgViews}", fallbackViews);
        
        return (fallbackViews, []);
    }

    private async Task<int> GetPremiumPercentageAsync(WTelegram.Client client, TL.Channel channel)
    {
        try
        {
            var stats = await client.Premium_GetBoostsStatus(channel);

            
            return (int)stats.premium_audience.part;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get premium percentage");
            return 0;
        }
    }

    private void UpdateChannelWithAnalytics(
        Database.Models.Channel channel,
        int subscribers,
        int averageViews,
        int premiumCount,
        List<LanguageDistributionContract> languages)
    {
        channel.UpdateAnalytics(subscribers, premiumCount, averageViews);
        
        if (languages.Count > 0)
        {
            channel.UpdateLanguageDistribution(languages);
        }
    }

    private async Task<int> CalculateAverageViewsFromMessagesAsync(WTelegram.Client client, TL.Channel channel)
    {
        try
        {
            var messages = await client.Messages_GetHistory(channel, limit: 100);
            var messagesWithViews = messages.Messages
                .OfType<Message>()
                .Where(m => m.views > 0)
                .ToList();

            return messagesWithViews.Count == 0 ? 0 : (int)messagesWithViews.Average(m => m.views);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to calculate average views from messages for channel {ChannelId}", channel.ID);
            return 0;
        }
    }

    private async Task<List<LanguageDistributionContract>> ParseLanguageDistributionAsync(
        WTelegram.Client client,
        Stats_BroadcastStats stats)
    {
        try
        {
            var statsGraph = await LoadLanguageGraphAsync(client, stats.languages_graph);
            if (statsGraph is null) return [];

            var (columns, names) = DeserializeGraphData(statsGraph);
            var languages = ExtractLanguagesFromGraph(columns, names);
            ConvertToPercentages(languages);

            logger.LogInformation("Parsed {Count} languages", languages.Count);
            return languages.OrderByDescending(x => x.Percentage).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to parse language distribution");
            return [];
        }
    }

    private async Task<StatsGraph?> LoadLanguageGraphAsync(WTelegram.Client client, StatsGraphBase? graphBase)
    {
        return graphBase switch
        {
            StatsGraphAsync asyncGraph => await client.Stats_LoadAsyncGraph(asyncGraph.token) as StatsGraph,
            StatsGraph directGraph => directGraph,
            _ => null
        };
    }

    private (JsonArray? columns, JsonObject? names) DeserializeGraphData(StatsGraph statsGraph)
    {
        var jsonData = JsonSerializer.Deserialize<JsonNode>(statsGraph.json.data);
        if (jsonData is null) return (null, null);

        var columns = jsonData["columns"]?.AsArray();
        var names = jsonData["names"]?.AsObject();

        return (columns, names);
    }

    private List<LanguageDistributionContract> ExtractLanguagesFromGraph(JsonArray? columns, JsonObject? names)
    {
        if (columns is null || names is null) return [];

        var languages = new List<LanguageDistributionContract>();

        foreach (var column in columns)
        {
            if (column is null) continue;

            var array = column.AsArray();
            if (array.Count < 2) continue;

            var columnKey = array[0]?.ToString();
            if (columnKey == "x") continue;

            var languageName = GetLanguageName(names, columnKey);
            var value = GetColumnValue(array);

            if (value > 0)
            {
                languages.Add(new LanguageDistributionContract
                {
                    Language = languageName,
                    Percentage = value
                });
            }
        }

        return languages;
    }

    private static string GetLanguageName(JsonObject names, string? columnKey)
    {
        if (string.IsNullOrEmpty(columnKey)) return "Unknown";
        
        return names.TryGetPropertyValue(columnKey, out var nameNode)
            ? nameNode?.ToString() ?? columnKey
            : columnKey;
    }

    private static double GetColumnValue(JsonArray array)
    {
        return double.TryParse(array.Last()?.ToString(), out var value) ? value : 0;
    }

    private static void ConvertToPercentages(List<LanguageDistributionContract> languages)
    {
        var total = languages.Sum(x => x.Percentage);
        if (total <= 0) return;

        foreach (var language in languages)
        {
            language.Percentage = (language.Percentage / total) * 100;
        }
    }
}
