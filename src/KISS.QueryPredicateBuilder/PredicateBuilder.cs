namespace KISS.QueryPredicateBuilder;

public sealed record PredicateBuilder<TEntity>
{
    public static WhereBuilder<TEntity> Filter { get; } = new();
    public static SelectBuilder<TEntity> Select { get; } = new();
    public static FetchBuilder Fetch { get; } = new();
    public static OffsetBuilder Offset { get; } = new();
    public static OrderByBuilder<TEntity> Sort => new();
}