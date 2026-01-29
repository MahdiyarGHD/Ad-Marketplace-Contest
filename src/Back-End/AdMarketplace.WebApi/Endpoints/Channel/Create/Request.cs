using AdMarketplace.Domain.Contracts.Common;

namespace AdMarketplace.Endpoints.Channel.Create;

public class Request
{
    public required long TelegramChannelId { get; set; }
    public required string Title { get; set; }
    public string? Username { get; set; }
    public string? Description { get; set; }
    public int SubscriberCount { get; set; }
    public int AverageViews { get; set; }
    public List<LanguageDistributionContract>? LanguageDistributionJson { get; set; }
    public Guid? CategoryId { get; set; }
}
