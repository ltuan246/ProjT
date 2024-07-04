namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

public sealed record CombinedFilterDefinition(string Separator, IComponent[] Components) : IComponent
{
    public void Accept(IVisitor visitor) => visitor.Visit(this);
}