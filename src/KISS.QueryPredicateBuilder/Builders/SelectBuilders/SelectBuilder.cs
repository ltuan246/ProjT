namespace KISS.QueryPredicateBuilder.Builders.SelectBuilders;

/// <summary>
/// Defines the select builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public sealed record SelectBuilder<TEntity>
{
    private static Type Entity => typeof(TEntity);
    private static IEnumerable<PropertyInfo> Properties => Entity.GetProperties();

    private List<string> Columns { get; } = [];
    private List<string> ExColumns { get; } = [];

    /// <summary>
    /// Adds one or more field names to be included in the results.
    /// </summary>
    /// <param name="field">The field name.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>The select builder.</returns>
    public SelectBuilder<TEntity> Include<TField>(Expression<Func<TEntity, TField>> field)
    {
        Columns.Add(new ExpressionFieldDefinition<TEntity, TField>(field));
        return this;
    }

    /// <summary>
    /// Adds one or more field names to be excluded from the results.
    /// </summary>
    /// <param name="field">The field name.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>The select builder.</returns>
    public SelectBuilder<TEntity> Exclude<TField>(Expression<Func<TEntity, TField>> field)
    {
        ExColumns.Add(new ExpressionFieldDefinition<TEntity, TField>(field));
        return this;
    }

    /// <summary>
    /// A builder for specifying which fields of an entity should return.
    /// </summary>
    /// <returns>The SELECT clause.</returns>
    public ProjectionDefinition Build()
        => new($"SELECT {string.Join(", ", GetFields()):raw} FROM {Entity.Name:raw}s");

    /// <summary>
    /// Get fields of a entity should return.
    /// </summary>
    /// <returns>The field names.</returns>
    private IEnumerable<string> GetFields()
        => Columns.Count switch
        {
            0 => Properties.Select(p => p.Name).Except(ExColumns),
            _ => Columns.Except(ExColumns)
        };
}
