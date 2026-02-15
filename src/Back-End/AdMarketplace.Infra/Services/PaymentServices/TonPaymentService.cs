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
    
    public async Task<bool> VerifyPayment(string txHash, decimal expectedAmountTon, string expectedMemo, string expectedSender)
    {
        var wallet = new Address(_tonSetting.BusinessWallet);
    
        var transactions = await _client.GetTransactions(wallet, 20); 

        if (transactions is not { Length: > 0 }) 
            return false;

        var payment = transactions.FirstOrDefault(t => t.TransactionId.Hash == txHash);

        if (payment.TransactionId.Hash != txHash) return false;

        var actualNano = long.Parse(payment.InMsg.Value.ToNano());
        var expectedNano = (long)(expectedAmountTon * 1_000_000_000);

        if (actualNano < expectedNano) return false;

        var actualSender = payment.InMsg.Source.ToString();
        var actualMemo = payment.InMsg.Message; 

        return actualSender == new Address(expectedSender).ToString() 
               && actualMemo == expectedMemo;
    }
}