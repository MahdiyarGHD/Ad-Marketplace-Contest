using HazelApp.Domain.Common.Schemas;

namespace AdMarketplace.Database.Models;

public class UserChannelConnection : IDateTimeSchema
{
    public Guid Id { get; private set; }
    public long ChatId { get; private set; }
    public string Title { get; set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    
    
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public static UserChannelConnection Create(
        string title,
        Guid userId,
        long chatId)
    {
        return new UserChannelConnection
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            ChatId = chatId,
            Title = title,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

