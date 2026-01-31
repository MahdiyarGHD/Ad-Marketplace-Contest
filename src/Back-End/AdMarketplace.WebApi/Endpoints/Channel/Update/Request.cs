using AdMarketplace.Domain.Contracts.Common;

namespace AdMarketplace.Endpoints.Channel.Update;

public class Request
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int SubscriberCount { get; set; }
    public int AverageViews { get; set; }
    public List<LanguageDistributionContract>? LanguageDistributionJson { get; set; }
    public required Guid CategoryId { get; set; }
}
