namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>SELECT DISTINCT</c> clause.
/// </summary>
/// <param name="Selector">The table columns.</param>
internal sealed record SelectDistinctComponent(Expression Selector) : IQueryComponent
{
    /// <summary>
    ///     Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    public void Accept(IVisitor visitor) => visitor.Visit(this);
}
