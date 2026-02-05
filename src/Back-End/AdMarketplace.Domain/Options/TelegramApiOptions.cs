namespace AdMarketplace.Domain.Options;

public class TelegramApiOptions
{
    public const string KeyName = "TelegramApi";
    
    public List<TelegramApiCredential> Credentials { get; set; } = [];
}

public class TelegramApiCredential
{
    public required string ApiId { get; set; }
    public required string ApiHash { get; set; }
}
