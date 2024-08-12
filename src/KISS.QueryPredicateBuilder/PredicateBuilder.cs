namespace KISS.QueryPredicateBuilder;

/// <summary>
/// A static helper class containing various builders.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public static class PredicateBuilder<TEntity>
{
    /// <summary>
    /// Defines the where builder type.
    /// </summary>
    public static WhereBuilder<TEntity> Filter => new();

    /// <summary>
    /// Defines the select builder type.
    /// </summary>
    public static SelectBuilder<TEntity> Select => new();

    /// <summary>
    /// Defines the fetch builder type.
    /// </summary>
    public static FetchBuilder Fetch => new();

    /// <summary>
    /// Defines the sort builder type.
    /// </summary>
    public static OrderByBuilder<TEntity> Sort => new();
}