namespace KISS.QueryPredicateBuilder.Builders.OrderByBuilders;

/// <summary>
/// Defines the sort builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public sealed class OrderByBuilder<TEntity>
{
    private List<string> Columns { get; } = [];

    /// <summary>
    /// Appends an ascending sort to the builder.
    /// </summary>
    /// <param name="field">Sorts the results by field.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>The sort builder.</returns>
    public OrderByBuilder<TEntity> Ascending<TField>(Expression<Func<TEntity, TField>> field)
    {
        Columns.Add($"{(string)new ExpressionFieldDefinition<TEntity, TField>(field)} ASC");
        return this;
    }

    /// <summary>
    /// Appends a descending sort to the builder.
    /// </summary>
    /// <param name="field">Sorts the results by field.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>The sort builder.</returns>
    public OrderByBuilder<TEntity> Descending<TField>(Expression<Func<TEntity, TField>> field)
    {
        Columns.Add($"{(string)new ExpressionFieldDefinition<TEntity, TField>(field)} DESC");
        return this;
    }

    /// <summary>
    /// A builder for specifying which fields of an entity should sort.
    /// </summary>
    /// <returns>The ORDER BY clause.</returns>
    public OrderByDefinition Build()
        => new($"ORDER BY {string.Join(", ", Columns):raw}");
}
