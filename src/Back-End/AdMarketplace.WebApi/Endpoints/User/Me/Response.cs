namespace AdMarketplace.Endpoints.User.Me;

public class Response
{
    public required Guid Id { get; set; }
    public long UserId { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
