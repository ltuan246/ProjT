namespace KISS.QueryPredicateBuilder;

public sealed record PredicateBuilder<TEntity>
{
    public static WhereBuilder<TEntity> Filter { get; } = new();
}