namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

public sealed record SingleItemAsArrayOperatorFilterDefinition(FormattableString Formattable) : IComponent
{
    public void Accept(IVisitor visitor) => visitor.Visit(this);
}