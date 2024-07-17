namespace KISS.QueryPredicateBuilder.Builders.OrderByBuilders;

public sealed record OrderByDefinition(FormattableString Formattable) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}