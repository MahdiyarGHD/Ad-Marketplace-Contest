using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Types;
using HazelApp.Domain.Common.Schemas;

namespace AdMarketplace.Database.Models;

public class Channel : IDateTimeSchema
{
    public Guid Id { get; private set; }
    public long ChatId { get; private set; }
    public string Title { get; private set; }
    public string? Username { get; private set; }
    public string? Description { get; private set; }
    public int SubscriberCount { get; private set; }
    public int PremiumCount { get; private set; }
    public int AverageViews { get; private set; }
    public List<LanguageDistributionContract>? LanguageDistributionJson { get; private set; }
    public ChannelStatusType Status { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    
    
    public Guid OwnerId { get; private set; }
    public User Owner { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public static Channel Create(
        long telegramChannelId,
        string title,
        string? username,
        string? description,
        int subscriberCount,
        int averageViews,
        List<LanguageDistributionContract>? languageDistributionJson,
        Guid ownerId,
        Guid? categoryId)
    {
        return new Channel
        {
            Id = Guid.CreateVersion7(),
            ChatId = telegramChannelId,
            Title = title,
            Username = username,
            Description = description,
            SubscriberCount = subscriberCount,
            AverageViews = averageViews,
            LanguageDistributionJson = languageDistributionJson,
            OwnerId = ownerId,
            CategoryId = categoryId,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Update(
        string title,
        string? description,
        int subscriberCount,
        int averageViews,
        List<LanguageDistributionContract>? languageDistributionJson,
        Guid? categoryId)
    {
        Title = title;
        Description = description;
        SubscriberCount = subscriberCount;
        AverageViews = averageViews;
        LanguageDistributionJson = languageDistributionJson;
        CategoryId = categoryId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetStatus(ChannelStatusType type)
    {
        Status = type;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
