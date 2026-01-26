namespace AdMarketplace.Domain.Options;

public class JwtOptions
{
    public const string KeyName = "Jwt";
    
    public required string SigningKey { get; set; }
    public required TimeSpan ExpireAt { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
}