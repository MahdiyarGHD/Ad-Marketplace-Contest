using AdMarketplace.Database;
using AdMarketplace.Domain.Types;
using ErrorOr;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Endpoints.Channel.Home;

public class Endpoint(AdMarketDbContext dbContext)
    : EndpointWithoutRequest<ErrorOr<Response>>
{
    private const int TopPicksCount = 15;
    private const int BrowseCategoriesCount = 10;
    private const int CategoryChannelsCount = 10;
    private const int RandomCategoriesCount = 2;

    public override void Configure()
    {
        Get("/api/channels/home");
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(CancellationToken ct)
    {
        var elements = new List<ElementBase>();

        var topPicksElement = await BuildTopPicksElementAsync(ct);
        if (topPicksElement != null)
            elements.Add(topPicksElement);

        var browseCategoriesElement = await BuildBrowseCategoriesElementAsync(ct);
        if (browseCategoriesElement != null)
            elements.Add(browseCategoriesElement);

        var randomCategoryElements = await BuildRandomCategoryElementsAsync(ct);
        elements.AddRange(randomCategoryElements);

        return new Response
        {
            Elements = elements
        };
    }

    private async Task<ElementBase?> BuildTopPicksElementAsync(CancellationToken ct)
    {
        var topChannels = await dbContext.Channels
            .Where(c => c.Status == ChannelStatusType.Ready)
            .OrderByDescending(c => c.SubscriberCount)
            .ThenByDescending(c => c.AverageViews)
            .Include(c => c.Category)
            .Take(TopPicksCount)
            .ToListAsync(ct);

        if (topChannels.Count == 0)
            return null;

        var channelItems = topChannels.Select(MapToChannelItem).ToList();

        return new ChannelElement
        {
            Icon = "🔥",
            Label = "Top Picks",
            Type = ElementType.Channel,
            Items = channelItems
        };
    }

    private async Task<ElementBase?> BuildBrowseCategoriesElementAsync(CancellationToken ct)
    {
        var categoryChannelCounts = await dbContext.Channels
            .Where(ch => ch.Status == ChannelStatusType.Ready && ch.CategoryId != null)
            .GroupBy(ch => ch.CategoryId)
            .Select(g => new { CategoryId = g.Key!.Value, Count = g.Count() })
            .ToListAsync(ct);

        var topCategories = await dbContext.Categories
            .Where(c => categoryChannelCounts.Select(cc => cc.CategoryId).Contains(c.Id))
            .ToListAsync(ct);

        var topCategoriesWithCounts = topCategories
            .Select(c => new
            {
                Category = c,
                ActiveChannelCount = categoryChannelCounts.FirstOrDefault(cc => cc.CategoryId == c.Id)?.Count ?? 0
            })
            .OrderByDescending(x => x.ActiveChannelCount)
            .ThenBy(x => x.Category.DisplayOrder)
            .Take(BrowseCategoriesCount)
            .ToList();

        if (!topCategoriesWithCounts.Any())
            return null;

        var categoryItems = topCategoriesWithCounts
            .Select(x => new CategoryItem
            {
                Id = x.Category.Id,
                Name = x.Category.Name,
                Icon = x.Category.Icon
            })
            .ToList();

        return new CategoryElement
        {
            Icon = "📂",
            Label = "Browse Categories",
            Type = ElementType.Category,
            Items = categoryItems
        };
    }

    private async Task<List<ElementBase>> BuildRandomCategoryElementsAsync(CancellationToken ct)
    {
        var elements = new List<ElementBase>();

        var categoriesWithActiveChannels = await dbContext.Channels
            .Where(ch => ch.Status == ChannelStatusType.Ready && ch.CategoryId != null)
            .Select(ch => ch.CategoryId!.Value)
            .Distinct()
            .ToListAsync(ct);

        var categoriesWithChannels = await dbContext.Categories
            .Where(c => categoriesWithActiveChannels.Contains(c.Id))
            .OrderBy(_ => Guid.NewGuid()) 
            .Take(RandomCategoriesCount)
            .ToListAsync(ct);

        foreach (var category in categoriesWithChannels)
        {
            var categoryChannels = await dbContext.Channels
                    .Where(c => c.CategoryId == category.Id && c.Status == ChannelStatusType.Ready)
                .OrderByDescending(c => c.SubscriberCount)
                .Include(c => c.Category)
                .Take(CategoryChannelsCount)
                .ToListAsync(ct);

            if (categoryChannels.Count != 0)
            {
                elements.Add(new ChannelElement
                {
                    Icon = category.Icon ?? "📁",
                    Label = category.Name,
                    Type = ElementType.Channel,
                    Items = categoryChannels.Select(MapToChannelItem).ToList()
                });
            }
        }

        return elements;
    }

    private static ChannelItem MapToChannelItem(Database.Models.Channel channel)
    {
        return new ChannelItem
        {
            Id = channel.Id,
            ChatId = channel.ChatId,
            Title = channel.Title,
            Username = channel.Username,
            SubscriberCount = channel.SubscriberCount,
            Category = new CategoryItem
            {
                Id = channel.Category?.Id ?? Guid.Empty,
                Name = channel.Category?.Name ?? "Uncategorized",
                Icon = channel.Category?.Icon
            }
        };
    }
}
