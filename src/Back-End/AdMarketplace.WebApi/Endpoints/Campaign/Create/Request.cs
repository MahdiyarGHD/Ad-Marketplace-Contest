using AdMarketplace.Domain.Contracts.Common;

namespace AdMarketplace.Endpoints.Campaign.Create;

public class Request
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal BudgetTon { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Brief { get; set; }
    public decimal? MaxPricePerPlacement { get; set; }
    public CampaignTargetingContract? Targeting { get; set; }
    public CampaignCreativeContract? Creative { get; set; }
    public DateTimeOffset? StartsAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
    public DateTimeOffset? ApplicationDeadline { get; set; }
}
