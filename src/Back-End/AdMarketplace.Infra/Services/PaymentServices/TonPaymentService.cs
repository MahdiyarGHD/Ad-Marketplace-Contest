using AdMarketplace.Domain.Options;
using Microsoft.Extensions.Options;
using TonSdk.Client;
using TonSdk.Contracts.Wallet;
using TonSdk.Core;
using TonSdk.Core.Block;
using TonSdk.Core.Boc;
using TonSdk.Core.Crypto;

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

        // var payment = transactions.FirstOrDefault(t => t.TransactionId.Hash == txHash);
        var payment = transactions.First();

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
    
    public async Task<string?> RefundAsync(string targetAddress, decimal amountTon, string memo)
    {
        var mnemonic = new Mnemonic(_tonSetting.Mnemonic.Split(' '));
        var wallet = new WalletV4(new WalletV4Options 
        { 
            PublicKey = mnemonic.Keys.PublicKey, 
            Workchain = 0 
        });

        var addressInfo = await _client.GetAddressInformation(wallet.Address);
        if (addressInfo is null)
            return null;
        
        var seqno = addressInfo.Value.Data == null ? 0 : wallet.ParseStorage(addressInfo.Value.Data.Parse()).Seqno;

        Cell body = new CellBuilder().StoreUInt(0, 32).StoreString(memo).Build();
        
        var info = new IntMsgInfo(new IntMsgInfoOptions
        {
            Dest = new Address(targetAddress),
            Value = new Coins(amountTon),
            Bounce = false
        });
        
        var messageOptions = new MessageXOptions
        {
            Info = info,
            Body = body
        };
        
        var message = new MessageX(messageOptions);

        var transfer = new WalletTransfer
        {
            Message = message,
            Mode = 3 
        };
        
        ExternalInMessage externalMessage = wallet.CreateTransferMessage(new[] { transfer }, seqno);

        var signedMessage = externalMessage.Sign(mnemonic.Keys.PrivateKey);

        Cell cellToSend = signedMessage.Cell; 

        var sendResult = await _client.SendBoc(cellToSend);

        if (sendResult is null)
            return null;
        
        return sendResult.Value.Type != "ok" ? throw new Exception($"Transaction rejected: {sendResult.Value.Type}") : cellToSend.Hash.ToString("hex");
    }
}