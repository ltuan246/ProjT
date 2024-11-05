namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>FROM</c> clause.
/// </summary>
/// <param name="Recordset">The type representing the database record set.</param>
internal sealed record SelectFromComponent(Type Recordset) : IQueryComponent
{
    /// <inheritdoc />
    public StringBuilder SqlBuilder { get; } = new();

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}
