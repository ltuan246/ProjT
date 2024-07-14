namespace KISS.QueryPredicateBuilder.Builders.SelectBuilders;

public sealed record ProjectionDefinition(FormattableString Formattable) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}