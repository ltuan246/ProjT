namespace KISS.QueryBuilder.Component;

public sealed record OperatorFilterDefinition<TComponent, TField>(
    ComparisonOperator Operator,
    FieldDefinition<TComponent, TField> Field,
    TField Value) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);

    public string Render() => CompositeQueries.Render(this);
}