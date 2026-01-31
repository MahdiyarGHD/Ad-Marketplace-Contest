using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Contracts.Responses;

namespace AdMarketplace.Endpoints.Channel.GetById;

public class Response
{
    public required Guid Id { get; set; }
    public required long ChatId { get; set; }
    public required string Title { get; set; }
    public string? Username { get; set; }
    public string? Description { get; set; }
    public int SubscriberCount { get; set; }
    public int AverageViews { get; set; }
    public List<LanguageDistributionContract>? LanguageDistributionJson { get; set; }
    public Guid OwnerId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<ChannelPricingResponseContract> Pricings { get; set; } = [];
}
