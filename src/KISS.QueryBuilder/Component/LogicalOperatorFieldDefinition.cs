namespace KISS.QueryBuilder.Component;

public sealed record LogicalOperatorFieldDefinition(params IFilterDefinition[] filterDefinitions) : IFilterDefinition
{
    public IEnumerable<IFilterDefinition> FilterDefinitions { get; } = filterDefinitions;

    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);

    public string Render() => CompositeQueries.Render(this);
}