namespace KISS.QueryBuilder.Component;

public sealed record AndFilterDefinition(params IComponent[] FilterDefinitions) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}