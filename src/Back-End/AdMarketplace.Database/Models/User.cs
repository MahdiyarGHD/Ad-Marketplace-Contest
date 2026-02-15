using HazelApp.Domain.Common.Schemas;

namespace AdMarketplace.Database.Models;

public class User : IDateTimeSchema
{
    public Guid Id { get; private set; }
    public long UserId { get; private set; }
    public string FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? UserName { get; private set; }
    public decimal Balance { get; set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    
    public static User Create(
        long userId,
        string firstName,
        string? userName,
        string? lastName)
    {
        return new User
        {
            Id = Guid.CreateVersion7(), 
            UserId =  userId,
            UserName =  userName,
            FirstName = firstName,
            LastName = lastName,
            
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}