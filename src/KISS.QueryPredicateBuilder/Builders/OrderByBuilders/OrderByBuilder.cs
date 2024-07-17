namespace KISS.QueryPredicateBuilder.Builders.OrderByBuilders;

public sealed record OrderByBuilder<TEntity>
{
    private List<string> Columns { get; } = [];

    public OrderByBuilder<TEntity> Ascending<TField>(Expression<Func<TEntity, TField>> field)
    {
        Columns.Add($"{(string)new ExpressionFieldDefinition<TEntity, TField>(field)} ASC");
        return this;
    }

    public OrderByBuilder<TEntity> Descending<TField>(Expression<Func<TEntity, TField>> field)
    {
        Columns.Add($"{(string)new ExpressionFieldDefinition<TEntity, TField>(field)} DESC");
        return this;
    }

    public OrderByDefinition Build()
        => new($"ORDER BY {string.Join(", ", Columns):raw}");
}