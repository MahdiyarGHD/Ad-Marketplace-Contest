using TonSdk.Client;

namespace AdMarketplace.Infra.Services.PaymentServices;

public class TonPaymentService
{
    private readonly TonClient _client;
    private readonly string _myBusinessWallet = "EQ...your_wallet_address...";

    public TonPaymentService(string apiKey, bool isTestnet = true)
    {
        var options = new HttpParameters
        {
            Endpoint = isTestnet ? "https://testnet.toncenter.com/api/v2/jsonRPC" : "https://toncenter.com/api/v2/jsonRPC",
            ApiKey = apiKey
        };
        
        _client = new TonClient(TonClientType.HTTP_TONCENTERAPIV3, options);
    }
}