namespace AdMarketplace.Domain.Contracts.Responses;

public class CategoryResponseDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
}