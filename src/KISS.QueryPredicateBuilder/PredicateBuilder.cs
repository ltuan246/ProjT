namespace KISS.QueryPredicateBuilder;

public sealed record PredicateBuilder<TEntity>
{
    public static WhereBuilder<TEntity> Filter { get; } = new();
    public static SelectBuilder<TEntity> Select { get; } = new();
}