namespace KISS.QueryBuilder.Component;

public sealed record OrFilterDefinition(params IQuerying[] FilterDefinitions) : IQuerying
{
    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}