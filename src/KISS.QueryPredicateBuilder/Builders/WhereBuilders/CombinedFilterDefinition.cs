namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

public sealed record CombinedFilterDefinition(ClauseAction Clause, string Separator, IComponent[] Components) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}