namespace KISS.QueryBuilder.Component;

public sealed record OrFilterDefinition(params IComponent[] FilterDefinitions) : IComponent
{
    void IComponent.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}