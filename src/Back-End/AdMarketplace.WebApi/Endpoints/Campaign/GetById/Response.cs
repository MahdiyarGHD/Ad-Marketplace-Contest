using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Campaign.GetById;

public class Response
{
    public required Guid Id { get; set; }
    public required Guid AdvertiserId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required decimal BudgetTon { get; set; }
    public CampaignStatusType Status { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Brief { get; set; }
    public decimal? MaxPricePerPlacement { get; set; }
    public CampaignTargetingContract? Targeting { get; set; }
    public CampaignCreativeContract? Creative { get; set; }
    public DateTimeOffset? StartsAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
    public DateTimeOffset? ApplicationDeadline { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
