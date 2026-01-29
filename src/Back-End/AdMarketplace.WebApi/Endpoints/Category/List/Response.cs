namespace AdMarketplace.Endpoints.Category.List;

public class Response
{
    public required List<CategoryItem> Categories { get; set; }
}

public class CategoryItem
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
}

