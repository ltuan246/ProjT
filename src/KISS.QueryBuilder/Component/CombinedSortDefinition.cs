namespace KISS.QueryBuilder.Component;

public sealed record CombinedSortDefinition<TEntity>(IEnumerable<DirectionalSortDefinition<TEntity>> Sorts)
{

}