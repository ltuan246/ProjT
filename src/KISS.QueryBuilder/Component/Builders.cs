namespace KISS.QueryBuilder.Component;

/// <summary>
/// A static helper class containing various builders.
/// </summary>
/// <typeparam name="TComponent">The type of the component.</typeparam>
public static class Builders<TComponent>
{
    public static FilterDefinitionBuilder<TComponent> Filter { get; } = new();

    public static SortDefinitionBuilder<TComponent> Sort { get; } = new();
}