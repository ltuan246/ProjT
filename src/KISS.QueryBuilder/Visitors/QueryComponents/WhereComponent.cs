namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>WHERE</c> clause.
/// </summary>
/// <param name="Predicate">Filters a sequence of values based on a predicate.</param>
internal sealed record WhereComponent(Expression Predicate) : IQueryComponent
{
    /// <inheritdoc />
    public StringBuilder SqlBuilder { get; } = new();

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}
