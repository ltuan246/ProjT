namespace KISS.QueryBuilder.Component;

public sealed record SingleItemAsArrayOperatorFilterDefinition<TComponent, TField>(
    SingleItemAsArrayOperator Operator,
    ExpressionFieldDefinition<TComponent, TField> Field,
    params TField[] Values) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}