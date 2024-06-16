namespace KISS.QueryBuilder.Component;

public class SortDefinitionBuilder<TEntity>
{
    /// <summary>
    /// Creates an ascending sort.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>An ascending sort.</returns>
    public DirectionalSortDefinition<TEntity> Ascending(FieldDefinition<TEntity> field)
        => new(field, SortDirection.Ascending);

    /// <summary>
    /// Creates an ascending sort.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>An ascending sort.</returns>
    public DirectionalSortDefinition<TEntity> Ascending<TField>(Expression<Func<TEntity, TField>> field)
        => Ascending(new ExpressionFieldDefinition<TEntity>(field));

    /// <summary>
    /// Creates a descending sort.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>A descending sort.</returns>
    public DirectionalSortDefinition<TEntity> Descending(FieldDefinition<TEntity> field)
        => new(field, SortDirection.Descending);

    /// <summary>
    /// Creates a descending sort.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>A descending sort.</returns>
    public DirectionalSortDefinition<TEntity> Descending<TField>(Expression<Func<TEntity, TField>> field)
        => Descending(new ExpressionFieldDefinition<TEntity>(field));

    /// <summary>
    /// Creates a combined sort.
    /// </summary>
    /// <param name="sorts">The sorts.</param>
    /// <returns>A combined sort.</returns>
    public CombinedSortDefinition<TEntity> Combine(params DirectionalSortDefinition<TEntity>[] sorts)
        => Combine((IEnumerable<DirectionalSortDefinition<TEntity>>)sorts);

    /// <summary>
    /// Creates a combined sort.
    /// </summary>
    /// <param name="sorts">The sorts.</param>
    /// <returns>A combined sort.</returns>
    public CombinedSortDefinition<TEntity> Combine(IEnumerable<DirectionalSortDefinition<TEntity>> sorts)
        => new(sorts);
}