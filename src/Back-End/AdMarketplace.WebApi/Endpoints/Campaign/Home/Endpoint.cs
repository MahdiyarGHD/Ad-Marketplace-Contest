using AdMarketplace.Database;
using AdMarketplace.Domain.Types;
using ErrorOr;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Endpoints.Campaign.Home;

public class Endpoint(AdMarketDbContext dbContext)
    : EndpointWithoutRequest<ErrorOr<Response>>
{
    private const int FeaturedCount = 15;
    private const int BrowseCategoriesCount = 10;
    private const int EndingSoonCount = 10;
    private const int NewCampaignsCount = 10;
    private const int CategoryCampaignsCount = 10;
    private const int RandomCategoriesCount = 2;

    public override void Configure()
    {
        Get("/api/campaigns/home");
        AllowAnonymous();
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(CancellationToken ct)
    {
        var elements = new List<ElementBase>();

        var featuredElement = await BuildFeaturedCampaignsElementAsync(ct);
        if (featuredElement != null)
            elements.Add(featuredElement);

        var browseCategoriesElement = await BuildBrowseCategoriesElementAsync(ct);
        if (browseCategoriesElement != null)
            elements.Add(browseCategoriesElement);

        var endingSoonElement = await BuildEndingSoonElementAsync(ct);
        if (endingSoonElement != null)
            elements.Add(endingSoonElement);

        var newCampaignsElement = await BuildNewCampaignsElementAsync(ct);
        if (newCampaignsElement != null)
            elements.Add(newCampaignsElement);

        var randomCategoryElements = await BuildRandomCategoryElementsAsync(ct);
        elements.AddRange(randomCategoryElements);

        return new Response
        {
            Elements = elements
        };
    }

    private async Task<ElementBase?> BuildFeaturedCampaignsElementAsync(CancellationToken ct)
    {
        var featuredCampaigns = await dbContext.Campaigns
            .Where(c => c.Status == CampaignStatusType.Active)
            .OrderByDescending(c => c.BudgetTon)
            .ThenByDescending(c => c.CreatedAt)
            .Include(c => c.Category)
            .Take(FeaturedCount)
            .ToListAsync(ct);

        if (featuredCampaigns.Count == 0)
            return null;

        var campaignItems = featuredCampaigns.Select(MapToCampaignItem).ToList();

        return new CampaignElement
        {
            Icon = "🔥",
            Label = "Featured Campaigns",
            Type = ElementType.Campaign,
            Items = campaignItems
        };
    }

    private async Task<ElementBase?> BuildBrowseCategoriesElementAsync(CancellationToken ct)
    {
        var categoryCampaignCounts = await dbContext.Campaigns
            .Where(c => c.Status == CampaignStatusType.Active && c.CategoryId != null)
            .GroupBy(c => c.CategoryId)
            .Select(g => new { CategoryId = g.Key!.Value, Count = g.Count() })
            .ToListAsync(ct);

        var topCategories = await dbContext.Categories
            .Where(c => categoryCampaignCounts.Select(cc => cc.CategoryId).Contains(c.Id))
            .ToListAsync(ct);

        var topCategoriesWithCounts = topCategories
            .Select(c => new
            {
                Category = c,
                Count = categoryCampaignCounts.First(cc => cc.CategoryId == c.Id).Count
            })
            .OrderByDescending(x => x.Count)
            .Take(BrowseCategoriesCount)
            .ToList();

        if (topCategoriesWithCounts.Count == 0)
            return null;

        var categoryItems = topCategoriesWithCounts.Select(x => new CategoryItem
        {
            Id = x.Category.Id,
            Name = x.Category.Name,
            Icon = x.Category.Icon,
            Count = x.Count
        }).ToList();

        return new CategoryElement
        {
            Icon = "📁",
            Label = "Browse Categories",
            Type = ElementType.Category,
            Items = categoryItems
        };
    }

    private async Task<ElementBase?> BuildEndingSoonElementAsync(CancellationToken ct)
    {
        var sevenDaysFromNow = DateTimeOffset.UtcNow.AddDays(7);

        var endingSoonCampaigns = await dbContext.Campaigns
            .Where(c => c.Status == CampaignStatusType.Active &&
                       c.ApplicationDeadline != null &&
                       c.ApplicationDeadline <= sevenDaysFromNow &&
                       c.ApplicationDeadline > DateTimeOffset.UtcNow)
            .OrderBy(c => c.ApplicationDeadline)
            .Include(c => c.Category)
            .Take(EndingSoonCount)
            .ToListAsync(ct);

        if (endingSoonCampaigns.Count == 0)
            return null;

        var campaignItems = endingSoonCampaigns.Select(MapToCampaignItem).ToList();

        return new CampaignElement
        {
            Icon = "⏰",
            Label = "Ending Soon",
            Type = ElementType.Campaign,
            Items = campaignItems
        };
    }

    private async Task<ElementBase?> BuildNewCampaignsElementAsync(CancellationToken ct)
    {
        var sevenDaysAgo = DateTimeOffset.UtcNow.AddDays(-7);

        var newCampaigns = await dbContext.Campaigns
            .Where(c => c.Status == CampaignStatusType.Active &&
                       c.CreatedAt >= sevenDaysAgo)
            .OrderByDescending(c => c.CreatedAt)
            .Include(c => c.Category)
            .Take(NewCampaignsCount)
            .ToListAsync(ct);

        if (newCampaigns.Count == 0)
            return null;

        var campaignItems = newCampaigns.Select(MapToCampaignItem).ToList();

        return new CampaignElement
        {
            Icon = "✨",
            Label = "New Campaigns",
            Type = ElementType.Campaign,
            Items = campaignItems
        };
    }

    private async Task<List<ElementBase>> BuildRandomCategoryElementsAsync(CancellationToken ct)
    {
        var categoryCampaignCounts = await dbContext.Campaigns
            .Where(c => c.Status == CampaignStatusType.Active && c.CategoryId != null)
            .GroupBy(c => c.CategoryId)
            .Select(g => new { CategoryId = g.Key!.Value, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(ct);

        if (categoryCampaignCounts.Count == 0)
            return new List<ElementBase>();

        var selectedCategories = categoryCampaignCounts
            .OrderBy(_ => Guid.NewGuid())
            .Take(RandomCategoriesCount)
            .ToList();

        var categoryIds = selectedCategories.Select(x => x.CategoryId).ToList();
        var categories = await dbContext.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync(ct);

        var elements = new List<ElementBase>();

        foreach (var selectedCategory in selectedCategories)
        {
            var category = categories.FirstOrDefault(c => c.Id == selectedCategory.CategoryId);
            if (category == null)
                continue;

            var categoryCampaigns = await dbContext.Campaigns
                .Where(c => c.Status == CampaignStatusType.Active &&
                           c.CategoryId == category.Id)
                .OrderByDescending(c => c.BudgetTon)
                .ThenByDescending(c => c.CreatedAt)
                .Include(c => c.Category)
                .Take(CategoryCampaignsCount)
                .ToListAsync(ct);

            if (categoryCampaigns.Count == 0)
                continue;

            var campaignItems = categoryCampaigns.Select(MapToCampaignItem).ToList();

            elements.Add(new CampaignElement
            {
                Icon = category.Icon ?? "📢",
                Label = $"{category.Name} Campaigns",
                Type = ElementType.Campaign,
                Items = campaignItems
            });
        }

        return elements;
    }

    private static CampaignItem MapToCampaignItem(Database.Models.Campaign campaign)
    {
        return new CampaignItem
        {
            Id = campaign.Id,
            Title = campaign.Title,
            Description = campaign.Description.Length > 150
                ? campaign.Description.Substring(0, 147) + "..."
                : campaign.Description,
            BudgetTon = campaign.BudgetTon,
            MaxPricePerPlacement = campaign.MaxPricePerPlacement,
            Status = campaign.Status,
            ApplicationDeadline = campaign.ApplicationDeadline,
            Category = campaign.Category != null
                ? new CategoryItem
                {
                    Id = campaign.Category.Id,
                    Name = campaign.Category.Name,
                    Icon = campaign.Category.Icon,
                    Count = 0 // Not used in campaign items
                }
                : null
        };
    }
}
