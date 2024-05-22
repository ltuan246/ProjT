namespace KISS.QueryBuilder.Component;

public sealed record SingleItemAsArrayOperatorFilterDefinition<TComponent, TField>(
    SingleItemAsArrayOperators Operator,
    FieldDefinition<TComponent, TField> Field,
    params TField[] Values) : IFilterDefinition
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);

    public string Render() => CompositeQueries.Render(this);
}