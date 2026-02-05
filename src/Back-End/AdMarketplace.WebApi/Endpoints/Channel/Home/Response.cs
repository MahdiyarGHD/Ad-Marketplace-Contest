using System;
using System.Text.Json.Serialization;

namespace AdMarketplace.Endpoints.Channel.Home;

public class Response
{
    public required List<ElementBase> Elements { get; set; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ChannelElement), typeDiscriminator: "channel")]
[JsonDerivedType(typeof(CategoryElement), typeDiscriminator: "category")]
public abstract class ElementBase
{
    public required string Icon { get; set; }
    public required string Label { get; set; }
    public required ElementType Type { get; set; }
}

public class ChannelElement : ElementBase
{
    public required List<ChannelItem> Items { get; set; } = [];
}

public class CategoryElement : ElementBase
{
    public required List<CategoryItem> Items { get; set; } = [];
}

public enum ElementType 
{
    Channel,
    Category
}

public class ChannelItem 
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string? Username { get; set; }
    public required int SubscriberCount { get; set; }
    public required CategoryItem Category { get; set; }
}

public class CategoryItem 
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string? Icon { get; set; }
}
