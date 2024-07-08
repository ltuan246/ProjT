namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

public sealed record RangeFilterDefinition(FormattableString Formattable) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}