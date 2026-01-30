using AdMarketplace.Domain.Contracts.Common;

namespace AdMarketplace.Endpoints.Channel.Create;

public class Response
{
    public required Guid Id { get; set; }
    public required long ChatId { get; set; }
    public required string Title { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
