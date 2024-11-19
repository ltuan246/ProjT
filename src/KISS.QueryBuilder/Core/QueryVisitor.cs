namespace KISS.QueryBuilder.Core;

/// <summary>
///     A class that defines the fluent SQL builder type.
/// </summary>
internal sealed record QueryVisitor : IVisitor
{
    /// <summary>
    ///     Gets the generated the SQL.
    /// </summary>
    public string Sql
        => SqlBuilder.ToString();

    private StringBuilder SqlBuilder { get; } = new();

    /// <inheritdoc />
    public void Visit(SelectComponent element)
    {
        SqlBuilder.Append(element.SqlBuilder);
    }

    /// <inheritdoc />
    public void Visit(SelectFromComponent element)
    {
        SqlBuilder.Append(element.SqlBuilder);
    }

    /// <inheritdoc />
    public void Visit(JoinComponent element)
    {
        SqlBuilder.Append(element.SqlBuilder);
    }

    /// <inheritdoc />
    public void Visit(WhereComponent element)
    {
        SqlBuilder.Append(element.SqlBuilder);
    }

    /// <inheritdoc />
    public void Visit(GroupByComponent element)
    {
    }

    /// <inheritdoc />
    public void Visit(OrderByComponent element)
    {
        SqlBuilder.Append(element.SqlBuilder);
    }

    /// <inheritdoc />
    public void Visit(LimitComponent element)
    {
        SqlBuilder.Append(element.SqlBuilder);
    }

    /// <inheritdoc />
    public void Visit(OffsetComponent element)
    {
        SqlBuilder.Append(element.SqlBuilder);
    }
}
