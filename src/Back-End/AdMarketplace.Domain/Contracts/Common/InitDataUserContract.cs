using System.Text.Json.Serialization;

namespace AdMarketplace.Domain.Contracts.Common;

public class InitDataUserContract
{
    public long Id { get; init; }
    public required string FirstName { get; init; }
    public string? LastName { get; init; }
    public required string LanguageCode { get; init; }
    public required string PhotoUrl { get; init; }
    
    [JsonPropertyName("username")]
    public string? UserName { get; init; }
}