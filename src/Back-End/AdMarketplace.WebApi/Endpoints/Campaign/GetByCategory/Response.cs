using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Campaign.GetByCategory;

public class Response
{
    public required List<CampaignItem> Campaigns { get; set; }
}

public class CampaignItem
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public decimal BudgetTon { get; set; }
    public decimal? MaxPricePerPlacement { get; set; }
    public CampaignStatusType Status { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public object? Targeting { get; set; }
    public DateTimeOffset? ApplicationDeadline { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

