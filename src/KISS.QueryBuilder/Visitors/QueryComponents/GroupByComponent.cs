namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>GROUP BY</c> clause, grouping records by a specified key.
/// </summary>
/// <param name="KeySelector">The selector for the column to use as the group key.</param>
internal sealed record GroupByComponent(Expression KeySelector)
    : IQueryComponent
{
    /// <summary>
    ///     Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    public void Accept(IVisitor visitor) => visitor.Visit(this);
}
