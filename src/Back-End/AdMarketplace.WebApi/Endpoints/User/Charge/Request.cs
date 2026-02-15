namespace AdMarketplace.Endpoints.User.Charge;

public class Request
{
    public string TransactionHash { get; set; }
    public string WalletAddress { get; set; }
}
