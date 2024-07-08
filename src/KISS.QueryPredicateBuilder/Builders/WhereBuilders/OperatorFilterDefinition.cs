namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

public sealed record OperatorFilterDefinition(FormattableString Formattable) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}