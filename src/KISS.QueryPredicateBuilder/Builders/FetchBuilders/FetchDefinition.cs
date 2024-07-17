namespace KISS.QueryPredicateBuilder.Builders.FetchBuilders;

public sealed record FetchDefinition(FormattableString Formattable) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);
}