using AdMarketplace.Domain.Types;

namespace AdMarketplace.Endpoints.Channel.Search;

public class Request
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
    public Guid? CategoryId { get; set; }
    public int? MinSubscribers { get; set; }
    public int? MaxSubscribers { get; set; }
    public int? MinAverageViews { get; set; }
    public AdFormatType? AdFormat { get; set; }
    public PriceType? PriceType { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Language { get; set; }
}
