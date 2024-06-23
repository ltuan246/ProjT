namespace KISS.QueryBuilder.Queries.Sorting;

public sealed record CombinedSortDefinition(IEnumerable<DirectionalSortDefinition> Sorts) : ICombinedSortDefinition
{
    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);
}