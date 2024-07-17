namespace KISS.QueryPredicateBuilder.Builders.OffsetBuilders;

public sealed record OffsetDefinition(FormattableString Formattable) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}