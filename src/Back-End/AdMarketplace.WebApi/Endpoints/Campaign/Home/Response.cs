using AdMarketplace.Domain.Types;
using System.Text.Json.Serialization;

namespace AdMarketplace.Endpoints.Campaign.Home;

public class Response
{
    public required List<ElementBase> Elements { get; set; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(CampaignElement), typeDiscriminator: "campaign")]
[JsonDerivedType(typeof(CategoryElement), typeDiscriminator: "category")]
public abstract class ElementBase
{
    public required string Icon { get; set; }
    public required string Label { get; set; }
    public required ElementType Type { get; set; }
}

public class CampaignElement : ElementBase
{
    public required List<CampaignItem> Items { get; set; } = [];
}

public class CategoryElement : ElementBase
{
    public required List<CategoryItem> Items { get; set; } = [];
}

public enum ElementType
{
    Campaign,
    Category
}

public class CampaignItem
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal BudgetTon { get; set; }
    public decimal? MaxPricePerPlacement { get; set; }
    public CampaignStatusType Status { get; set; }
    public DateTimeOffset? ApplicationDeadline { get; set; }
    public CategoryItem? Category { get; set; }
}

public class CategoryItem
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public int Count { get; set; }
}
