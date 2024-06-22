namespace KISS.QueryBuilder.Queries.Sorting;

public sealed record MultipleSortsDefinition(IEnumerable<DirectionalSortDefinition> Sorts) : IMultipleSortsDefinition
{
    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);
}