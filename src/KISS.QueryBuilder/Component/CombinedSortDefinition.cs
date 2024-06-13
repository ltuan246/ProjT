namespace KISS.QueryBuilder.Component;

public sealed record CombinedSortDefinition<TComponent>(IEnumerable<DirectionalSortDefinition<TComponent>> Sorts)
{

}