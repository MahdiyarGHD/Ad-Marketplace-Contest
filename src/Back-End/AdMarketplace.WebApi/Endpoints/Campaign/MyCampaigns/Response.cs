using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Campaign.MyCampaigns;

public class Response
{
    public required List<CampaignItem> Campaigns { get; set; }
}

public class CampaignItem
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal BudgetTon { get; set; }
    public CampaignStatusType Status { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public CampaignTargetingContract? Targeting { get; set; }
    public DateTimeOffset? ApplicationDeadline { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
