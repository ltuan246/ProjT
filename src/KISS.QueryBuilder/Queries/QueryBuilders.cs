namespace KISS.QueryBuilder.Queries;

public sealed record QueryBuilders(IEnumerable<IQuerying> Queries) : IQueryBuilders
{
    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries.Render(this);
}