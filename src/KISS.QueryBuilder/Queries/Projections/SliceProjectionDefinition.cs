namespace KISS.QueryBuilder.Queries.Projections;

public sealed record SliceProjectionDefinition(int Limit) : ISliceProjectionDefinition
{
    public int Skip { get; init; }

    public SliceProjectionDefinition(int limit, int skip) : this(limit)
    {
        Skip = skip;
    }

    void IQuerying.Accept(IVisitor visitor) => visitor.Visit(this);
}