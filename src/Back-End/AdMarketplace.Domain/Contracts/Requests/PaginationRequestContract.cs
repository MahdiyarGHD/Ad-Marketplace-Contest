namespace AdMarketplace.Domain.Contracts.Requests;

public class PaginationRequestContract
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
}

