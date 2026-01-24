namespace AdMarketplace.Domain.Contracts.Common;

public class InitDataUserContract
{
    public long Id { get; init; }
    public required string Token { get; set; }
    public required string FirstName { get; init; }
    public string? LastName { get; init; }
    public required string LanguageCode { get; init; }
    public required string PhotoUrl { get; init; }
}