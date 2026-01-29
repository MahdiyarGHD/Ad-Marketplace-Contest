using System.Text.Json.Serialization;

namespace AdMarketplace.Domain.Contracts.Common;

public class InitDataUserContract
{
    public long Id { get; init; }
    public string FirstName { get; init; }
    public string? LastName { get; init; }
    public string LanguageCode { get; init; }
    public string PhotoUrl { get; init; }
    
    [JsonPropertyName("username")]
    public string? UserName { get; init; }
}