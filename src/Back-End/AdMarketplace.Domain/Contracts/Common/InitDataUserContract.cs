namespace AdMarketplace.Domain.Contracts.Common;

public class InitDataUserContract
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public required string LanguageCode { get; set; }
    public required string PhotoUrl { get; set; }
}