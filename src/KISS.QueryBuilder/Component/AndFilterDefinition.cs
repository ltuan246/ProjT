namespace KISS.QueryBuilder.Component;

public sealed record AndFilterDefinition(params IQuerying[] FilterDefinitions) : IQuerying
{
    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}