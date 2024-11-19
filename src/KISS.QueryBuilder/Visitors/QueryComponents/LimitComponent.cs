namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>LIMIT</c> clause.
/// </summary>
internal sealed record LimitComponent : IQueryComponent
{
    /// <inheritdoc />
    public StringBuilder SqlBuilder { get; } = new();

    /// <summary>
    ///     The maximum number of rows to return.
    /// </summary>
    public int Rows { get; set; }

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        if (Rows > 0)
        {
            SqlBuilder.Append($"LIMIT {Rows}");
            SqlBuilder.AppendLine();
        }

        visitor.Visit(this);
    }
}
