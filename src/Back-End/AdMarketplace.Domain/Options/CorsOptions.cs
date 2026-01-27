namespace AdMarketplace.Domain.Options;

public class CorsOptions
{
    public const string KeyName = "Cors";
    public const string PolicyName = "Cors";

    public List<string> Origins { get; init; } = [];
}
