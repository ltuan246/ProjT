namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

public sealed record SingleItemAsArrayOperatorFilterDefinition(FormattableString Formattable) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}