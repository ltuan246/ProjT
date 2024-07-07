namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

public sealed record CombinedFilterDefinition(ClauseAction Clause, string Separator, IComponent[] Components) : IComponent
{
    public void Accept(IVisitor visitor) => visitor.Visit(this);
}