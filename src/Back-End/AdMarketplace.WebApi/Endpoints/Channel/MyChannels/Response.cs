using AdMarketplace.Domain.Contracts.Common;

namespace AdMarketplace.Endpoints.Channel.MyChannels;

public class Response
{
    public required List<ChannelItem> Channels { get; set; }
}

public class ChannelItem
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
    public DateTimeOffset CreatedAt { get; set; }
}
