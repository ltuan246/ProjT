namespace KISS.QueryBuilder.Queries;

public sealed record Builder<TEntity>(IEnumerable<IQuerying> Queries) : IBuilder
{
    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);

    public (string, Dictionary<string, object>) Render() => CompositeQueries<TEntity>.Render(this);
}