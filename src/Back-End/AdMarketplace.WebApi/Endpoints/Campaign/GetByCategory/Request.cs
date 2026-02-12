using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Domain.Contracts.Requests;

namespace AdMarketplace.Endpoints.Campaign.GetByCategory;

public class Request : PaginationRequestContract
{
    public Guid CategoryId { get; set; }
}

