namespace KISS.QueryBuilder.Component;

public sealed record ComparisonOperatorFilterDefinition<TComponent, TField>(
    ComparisonOperators Operator,
    FieldDefinition<TComponent, TField> Field,
    TField Value) : IFilterDefinition
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);

    public string Render() => CompositeQueries.Render(this);
}