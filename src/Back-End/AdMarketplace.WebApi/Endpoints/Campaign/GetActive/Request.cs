using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Campaign.GetActive;

public class Request
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
    public Guid? CategoryId { get; set; }
    public decimal? MinBudget { get; set; }
    public decimal? MaxBudget { get; set; }
    public AdFormatType? AdFormat { get; set; }
    public PriceType? PriceType { get; set; }
}
