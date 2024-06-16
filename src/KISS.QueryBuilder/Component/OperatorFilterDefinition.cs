namespace KISS.QueryBuilder.Component;

public sealed record OperatorFilterDefinition<TEntity, TField>(
    ComparisonOperator Operator,
    RenderedFieldDefinition Field,
    TField Value) : IFilterDefinition<TEntity>
{
    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}