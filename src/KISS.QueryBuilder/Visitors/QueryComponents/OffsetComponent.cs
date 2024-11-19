namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>OFFSET</c> clause.
/// </summary>
internal sealed record OffsetComponent : IQueryComponent
{
    /// <inheritdoc />
    public StringBuilder SqlBuilder { get; } = new();

    /// <summary>
    ///     The number of rows to skip before starting to return rows.
    /// </summary>
    public int Offset { get; set; }

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        if (Offset > 0)
        {
            SqlBuilder.Append($"OFFSET {Offset}");
            SqlBuilder.AppendLine();
        }

        visitor.Visit(this);
    }
}
