using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Campaign.Search;

public class Request
{
    public string? Query { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
    public Guid? CategoryId { get; set; }
    public decimal? MinBudget { get; set; }
    public decimal? MaxBudget { get; set; }
    public CampaignStatusType? Status { get; set; }
    public AdFormatType? AdFormat { get; set; }
    public PriceType? PriceType { get; set; }
}
