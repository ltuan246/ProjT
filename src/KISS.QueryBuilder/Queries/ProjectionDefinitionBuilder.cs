namespace KISS.QueryBuilder.Queries;

public sealed record ProjectionDefinitionBuilder<TEntity>
{
    public SingleFieldProjectionDefinition Include<TField>(Expression<Func<TEntity, TField>> field)
        => new((new ExpressionFieldDefinition<TEntity, TField>(field), true));

    public SingleFieldProjectionDefinition Exclude<TField>(Expression<Func<TEntity, TField>> field)
        => new((new ExpressionFieldDefinition<TEntity, TField>(field), false));

    public SliceProjectionDefinition Slice(int limit)
        => new(limit);

    public SliceProjectionDefinition Slice(int limit, int skip)
        => new(limit, skip);

    public CombinedProjectionDefinition Combine(params IQuerying[] projections)
        => new(projections);
}