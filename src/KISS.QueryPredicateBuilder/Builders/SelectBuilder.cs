namespace KISS.QueryPredicateBuilder.Builders;

public sealed record SelectBuilder<TEntity>
{
    private List<string> Columns { get; } = [];
    private List<string> ExColumns { get; } = [];

    public SelectBuilder<TEntity> Include<TField>(Expression<Func<TEntity, TField>> field)
    {
        Columns.Add(new ExpressionFieldDefinition<TEntity, TField>(field));
        return this;
    }

    public SelectBuilder<TEntity> Exclude<TField>(Expression<Func<TEntity, TField>> field)
    {
        ExColumns.Add(new ExpressionFieldDefinition<TEntity, TField>(field));
        return this;
    }

    public static implicit operator ProjectionDefinition(SelectBuilder<TEntity> selectBuilder)
        => new($"");
}