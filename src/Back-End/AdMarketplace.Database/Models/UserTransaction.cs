namespace AdMarketplace.Database.Models;

public class UserTransaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public User User { get; set; }

    public string TransactionHash { get; set; }
    public long LogicalTime { get; set; }
    public decimal Amount { get; set; }
    public string Destination { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    
    public static UserTransaction Create(
        string transactionHash,
        long logicalTime,
        Guid userId,
        decimal amount,
        string destination)
    {
        return new UserTransaction
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Amount =  amount,
            Destination = destination,  
            TransactionHash = transactionHash,
            LogicalTime = logicalTime,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}