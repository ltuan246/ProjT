namespace KISS.QueryBuilder.Component;

public class SortDefinitionBuilder<TComponent>
{
    /// <summary>
    /// Creates an ascending sort.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>An ascending sort.</returns>
    public DirectionalSortDefinition<TComponent> Ascending(FieldDefinition<TComponent> field)
        => new(field, SortDirection.Ascending);

    /// <summary>
    /// Creates an ascending sort.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>An ascending sort.</returns>
    public DirectionalSortDefinition<TComponent> Ascending<TField>(Expression<Func<TComponent, TField>> field)
        => Ascending(new ExpressionFieldDefinition<TComponent>(field));

    /// <summary>
    /// Creates a descending sort.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>A descending sort.</returns>
    public DirectionalSortDefinition<TComponent> Descending(FieldDefinition<TComponent> field)
        => new(field, SortDirection.Descending);

    /// <summary>
    /// Creates a descending sort.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>A descending sort.</returns>
    public DirectionalSortDefinition<TComponent> Descending<TField>(Expression<Func<TComponent, TField>> field)
        => Descending(new ExpressionFieldDefinition<TComponent>(field));

    /// <summary>
    /// Creates a combined sort.
    /// </summary>
    /// <param name="sorts">The sorts.</param>
    /// <returns>A combined sort.</returns>
    public CombinedSortDefinition<TComponent> Combine(params DirectionalSortDefinition<TComponent>[] sorts)
        => Combine((IEnumerable<DirectionalSortDefinition<TComponent>>)sorts);

    /// <summary>
    /// Creates a combined sort.
    /// </summary>
    /// <param name="sorts">The sorts.</param>
    /// <returns>A combined sort.</returns>
    public CombinedSortDefinition<TComponent> Combine(IEnumerable<DirectionalSortDefinition<TComponent>> sorts)
        => new(sorts);
}