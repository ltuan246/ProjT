namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>JOIN</c> clause.
/// </summary>
/// <param name="Relation">The type of table that want to join.</param>
/// <param name="MapSelector">The results of the joined tables to be mapped back into the selector.</param>
/// <param name="LeftKeySelector">The table as the left key.</param>
/// <param name="RightKeySelector">The table as the right key.</param>
internal sealed record JoinComponent(
    Type Relation,
    Expression MapSelector,
    Expression LeftKeySelector,
    Expression RightKeySelector)
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
