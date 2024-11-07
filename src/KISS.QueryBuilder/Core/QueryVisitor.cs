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
    }

    /// <inheritdoc />
    public void Visit(JoinComponent element)
    {
    }

    /// <inheritdoc />
    public void Visit(WhereComponent element)
    {
    }

    /// <inheritdoc />
    public void Visit(GroupByComponent element)
    {
    }
}
