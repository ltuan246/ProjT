namespace KISS.QueryBuilder.Component;

public class SortDefinitionBuilder<TEntity>
{
    /// <summary>
    /// Creates an ascending sort.
    /// </summary>
    /// <param name="fieldDefinition">The field.</param>
    /// <returns>An ascending sort.</returns>
    public DirectionalSortDefinition Ascending(RenderedFieldDefinition fieldDefinition)
        => new(SortDirection.Ascending, fieldDefinition);

    /// <summary>
    /// Creates an ascending sort.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>An ascending sort.</returns>
    public DirectionalSortDefinition Ascending<TField>(Expression<Func<TEntity, TField>> field)
        => Ascending(new ExpressionFieldDefinition<TEntity, TField>(field));

    /// <summary>
    /// Creates a descending sort.
    /// </summary>
    /// <param name="fieldDefinition">The field.</param>
    /// <returns>A descending sort.</returns>
    public DirectionalSortDefinition Descending(RenderedFieldDefinition fieldDefinition)
        => new(SortDirection.Descending, fieldDefinition);

    /// <summary>
    /// Creates a descending sort.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>A descending sort.</returns>
    public DirectionalSortDefinition Descending<TField>(Expression<Func<TEntity, TField>> field)
        => Descending(new ExpressionFieldDefinition<TEntity, TField>(field));

    /// <summary>
    /// Creates a combined sort.
    /// </summary>
    /// <param name="sorts">The sorts.</param>
    /// <returns>A combined sort.</returns>
    public MultipleSortsDefinition Combine(params DirectionalSortDefinition[] sorts)
        => Combine((IEnumerable<DirectionalSortDefinition>)sorts);

    /// <summary>
    /// Creates a combined sort.
    /// </summary>
    /// <param name="sorts">The sorts.</param>
    /// <returns>A combined sort.</returns>
    public MultipleSortsDefinition Combine(IEnumerable<DirectionalSortDefinition> sorts)
        => new(sorts);
}