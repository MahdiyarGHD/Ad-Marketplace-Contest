namespace AdMarketplace.Domain.Options;

public class TelegramBotOptions
{
    public const string KeyName = "TelegramBot";
    
    public string Token { get; set; }
    public string BotApiServer { get; set; }
    public string WebhookUrl { get; set; }
    public string SecretToken { get; set; }
}