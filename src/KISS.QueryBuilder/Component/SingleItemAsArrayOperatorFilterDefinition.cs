namespace KISS.QueryBuilder.Component;

public sealed record SingleItemAsArrayOperatorFilterDefinition<TComponent, TField>(
    SingleItemAsArrayOperator Operator,
    FieldDefinition<TComponent, TField> Field,
    params TField[] Values) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);

    public string Render() => CompositeQueries.Render(this);
}