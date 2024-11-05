namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>SELECT DISTINCT</c> clause.
/// </summary>
/// <param name="Selector">The table columns.</param>
internal sealed record SelectDistinctComponent(Expression Selector) : IQueryComponent
{
    /// <inheritdoc />
    public StringBuilder SqlBuilder { get; } = new();

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}
