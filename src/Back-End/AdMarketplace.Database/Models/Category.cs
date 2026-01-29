using HazelApp.Domain.Common.Schemas;

namespace AdMarketplace.Database.Models;

public class Category : IDateTimeSchema
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Icon { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Category Create(
        string name,
        string? description,
        string? icon,
        int displayOrder)
    {
        return new Category
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Description = description,
            Icon = icon,
            DisplayOrder = displayOrder,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static Category CreateWithId(
        Guid id,
        string name,
        string? description,
        string? icon,
        int displayOrder)
    {
        return new Category
        {
            Id = id,
            Name = name,
            Description = description,
            Icon = icon,
            DisplayOrder = displayOrder,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Update(string name, string? description, string? icon, int displayOrder)
    {
        Name = name;
        Description = description;
        Icon = icon;
        DisplayOrder = displayOrder;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

