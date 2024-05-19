namespace KISS.QueryBuilder.Component;

public sealed record Builders<TComponent>
{
    public static FilterDefinitionBuilder<TComponent> Filter { get; } = new();
}