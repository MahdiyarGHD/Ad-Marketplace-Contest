using AdMarketplace.Domain.Options;
using Microsoft.Extensions.Options;
using TonSdk.Client;
using TonSdk.Core;

namespace AdMarketplace.Infra.Services.PaymentServices;

public class TonPaymentService
{
    private readonly TonClient _client;
    private readonly TonSettingOptions _tonSetting;

    public TonPaymentService(IOptions<TonSettingOptions> tonSetting)
    {
        _tonSetting = tonSetting.Value;
        
        var options = new HttpParameters
        {
            Endpoint = _tonSetting.IsTestnet ? "https://testnet.toncenter.com/api/v2/jsonRPC" : "https://toncenter.com/api/v2/jsonRPC",
            ApiKey = _tonSetting.ApiKey
        };
        _client = new TonClient(TonClientType.HTTP_TONCENTERAPIV2, options);
    }
    
    public async Task<decimal?> GetDepositAmountAsync(string txHash, string expectedMemo, string expectedSender)
    {
        var wallet = new Address(_tonSetting.BusinessWallet);
    
        var transactions = await _client.GetTransactions(wallet, 20); 

        if (transactions is not { Length: > 0 }) 
            return null;

        var payment = transactions.FirstOrDefault(t => t.TransactionId.Hash == txHash);

        if (payment.TransactionId.Hash != txHash) 
            return null;

        var actualSender = payment.InMsg.Source.ToString();
        var actualMemo = payment.InMsg.Message;

        if (actualSender != new Address(expectedSender).ToString())
            return null;

        if (actualMemo != expectedMemo)
            return null;
        
        var actualNano = long.Parse(payment.InMsg.Value.ToNano());

        return actualNano;
    }
}