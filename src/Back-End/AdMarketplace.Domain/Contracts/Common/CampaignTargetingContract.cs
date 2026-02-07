using AdMarketplace.Domain.Types;

namespace AdMarketplace.Domain.Contracts.Common;

public class CampaignTargetingContract
{
    public int? MinSubscribers { get; set; }
    public int? MaxSubscribers { get; set; }
    public int? MinAverageViews { get; set; }
    public int? MinPremiumCount { get; set; }
    public List<Guid>? PreferredCategoryIds { get; set; }
    public List<string>? PreferredLanguages { get; set; }
    public List<AdFormatType>? PreferredAdFormats { get; set; }
    public List<PriceType>? PreferredPriceTypes { get; set; }
}
