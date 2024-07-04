namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

public sealed record RangeFilterDefinition(FormattableString Formattable) : IComponent
{
    public void Accept(IVisitor visitor) => visitor.Visit(this);
}