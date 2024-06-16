namespace KISS.QueryBuilder.Component;

/// <summary>
/// A static helper class containing various builders.
/// </summary>
/// <typeparam name="TEntity">The type of the component.</typeparam>
public static class Builders<TEntity>
{
    public static FilterDefinitionBuilder<TEntity> Filter { get; } = new();

    public static SortDefinitionBuilder<TEntity> Sort { get; } = new();
}