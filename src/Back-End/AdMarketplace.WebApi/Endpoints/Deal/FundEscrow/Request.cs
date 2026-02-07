namespace AdMarketplace.Endpoints.Deal.FundEscrow;

public class Request
{
    public Guid Id { get; set; }
    public required string TransactionHash { get; set; }
    public required string WalletAddress { get; set; }
}
