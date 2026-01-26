using HazelApp.Domain.Common.Schemas;

namespace AdMarketplace.Database.Models;

public class Agent : IDateTimeSchema
{
    public Guid Id { get; private set; }
    public long UserId { get; private set; }
    public string FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? UserName { get; private set; }
    public string SessionPath { get; private set; }
    public string PhoneNumber { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Agent Create(
        long userId,
        string phoneNumber,
        string firstName,
        string sessionPath,
        string? userName,
        string? lastName)
    {
        return new Agent
        {
            Id = Guid.CreateVersion7(), 
            UserId =  userId,
            PhoneNumber = phoneNumber,
            UserName =  userName,
            SessionPath = sessionPath,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}