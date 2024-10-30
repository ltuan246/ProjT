namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>WHERE</c> clause.
/// </summary>
/// <param name="Predicate">Filters a sequence of values based on a predicate.</param>
internal sealed record WhereComponent(Expression Predicate) : IQueryComponent
{
    /// <summary>
    ///     Let the visitor know the class of the component it works with.
    /// </summary>
    /// <param name="visitor">The Visitor declares a set of visiting methods that correspond to component classes.</param>
    public void Accept(IVisitor visitor) => visitor.Visit(this);
}
