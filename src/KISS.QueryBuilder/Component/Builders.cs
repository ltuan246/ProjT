namespace KISS.QueryBuilder.Component;

public static class Builders<TComponent>
{
    public static FilterDefinitionBuilder<TComponent> Filter { get; } = new();
}