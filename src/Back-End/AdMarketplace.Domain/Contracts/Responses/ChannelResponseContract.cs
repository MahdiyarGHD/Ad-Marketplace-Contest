using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Types;

namespace AdMarketplace.Domain.Contracts.Responses;

public class ChannelResponseContract
{
    public Guid Id { get; set; }
    public long ChatId { get; set; }
    public string Title { get; set; }
    public string? Username { get; set; }
    public string? Description { get; set; }
    public int SubscriberCount { get; set; }
    public int PremiumCount { get; set; }
    public int AverageViews { get; set; }
    public List<LanguageDistributionContract>? LanguageDistributionJson { get; set; }
    public ChannelStatusType Status { get; set; }
    public List<ChannelPricingResponseContract> Pricings { get; set; } = [];
}