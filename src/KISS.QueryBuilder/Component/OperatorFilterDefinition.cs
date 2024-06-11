namespace KISS.QueryBuilder.Component;

public sealed record OperatorFilterDefinition<TComponent, TField>(
    ComparisonOperator Operator,
    ExpressionFieldDefinition<TComponent, TField> Field,
    TField Value) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}