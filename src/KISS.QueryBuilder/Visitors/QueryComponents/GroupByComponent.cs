namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>GROUP BY</c> clause, grouping records by a specified key.
/// </summary>
/// <param name="KeySelector">The selector for the column to use as the group key.</param>
internal sealed record GroupByComponent(Expression KeySelector)
    : IQueryComponent
{
    /// <inheritdoc />
    public StringBuilder SqlBuilder { get; } = new();

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}
