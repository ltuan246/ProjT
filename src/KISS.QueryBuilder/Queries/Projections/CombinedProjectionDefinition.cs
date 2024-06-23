namespace KISS.QueryBuilder.Queries.Projections;

public sealed record CombinedProjectionDefinition(IQuerying[] Projections)
    : ICombinedProjectionDefinition
{
    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);
}