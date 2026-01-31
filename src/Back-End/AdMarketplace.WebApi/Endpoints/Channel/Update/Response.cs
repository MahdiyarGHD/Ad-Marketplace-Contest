using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Contracts.Responses;

namespace AdMarketplace.Endpoints.Channel.Update;

public class Response
{
    public required Guid Id { get; set; }
    public required long TelegramChannelId { get; set; }
    public required string Title { get; set; }
    public string? Username { get; set; }
    public string? Description { get; set; }
    public int SubscriberCount { get; set; }
    public int AverageViews { get; set; }
    public List<LanguageDistributionContract>? LanguageDistributionJson { get; set; }
    public bool IsBotAdmin { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<ChannelPricingResponseContract> Pricings { get; set; } = [];
}
