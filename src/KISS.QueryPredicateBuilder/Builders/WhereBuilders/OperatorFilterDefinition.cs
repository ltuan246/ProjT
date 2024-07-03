namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

public sealed class OperatorFilterDefinition : IComponent
{
    public void Accept(IVisitor visitor) => visitor.Visit(this);
}