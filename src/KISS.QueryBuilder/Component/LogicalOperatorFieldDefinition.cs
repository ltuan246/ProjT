namespace KISS.QueryBuilder.Component;

public sealed record LogicalOperatorFieldDefinition(
    LogicalOperators Operator,
    params IFilterDefinition[] FilterDefinitions) : IFilterDefinition
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);

    public string Render() => CompositeQueries.Render(this);
}