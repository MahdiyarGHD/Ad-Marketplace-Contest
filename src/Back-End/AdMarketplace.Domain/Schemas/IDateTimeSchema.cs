namespace HazelApp.Domain.Common.Schemas;

public interface IDateTimeSchema
{
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? UpdatedAt { get; }
}