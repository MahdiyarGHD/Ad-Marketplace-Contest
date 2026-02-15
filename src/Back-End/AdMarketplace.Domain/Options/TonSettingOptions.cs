namespace AdMarketplace.Domain.Options;

public class TonSettingOptions
{
    public const string KeyName = "TonSettings";
    
    public string ApiKey { get; set; }
    public bool IsTestnet { get; set; }
    public string BusinessWallet { get; set; }
    public string Mnemonic { get; set; }
}