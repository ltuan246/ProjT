namespace KISS.QueryBuilder.Queries;

public class SortDefinitionBuilder<TEntity>
{
    /// <summary>
    /// Creates an ascending sort.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>An ascending sort.</returns>
    public DirectionalSortDefinition Ascending<TField>(Expression<Func<TEntity, TField>> field)
        => new(SortDirection.Ascending, new ExpressionFieldDefinition<TEntity, TField>(field));

    /// <summary>
    /// Creates a descending sort.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>A descending sort.</returns>
    public DirectionalSortDefinition Descending<TField>(Expression<Func<TEntity, TField>> field)
        => new(SortDirection.Descending, new ExpressionFieldDefinition<TEntity, TField>(field));

    /// <summary>
    /// Creates a combined sort.
    /// </summary>
    /// <param name="sorts">The sorts.</param>
    /// <returns>A combined sort.</returns>
    public CombinedSortDefinition Combine(params DirectionalSortDefinition[] sorts)
        => new(sorts);
}