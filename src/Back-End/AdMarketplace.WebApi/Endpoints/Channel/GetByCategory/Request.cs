using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Contracts.Requests;

namespace AdMarketplace.Endpoints.Channel.GetByCategory;

public class Request : PaginationRequestContract
{
    public required Guid CategoryId { get; set; }
}
