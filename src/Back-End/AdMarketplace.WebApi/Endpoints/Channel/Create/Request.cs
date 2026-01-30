using AdMarketplace.Domain.Contracts.Common;

namespace AdMarketplace.Endpoints.Channel.Create;

public class Request
{
    public required long ChatId { get; set; }
    public required Guid CategoryId { get; set; }
}
