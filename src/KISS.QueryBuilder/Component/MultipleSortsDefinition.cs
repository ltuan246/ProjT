namespace KISS.QueryBuilder.Component;

public sealed record MultipleSortsDefinition(IEnumerable<DirectionalSortDefinition> Sorts)
{
}