namespace KISS.QueryBuilder.Core;

/// <summary>
///     The Visitor implements several versions of the same behaviors, tailored for different concrete element classes.
/// </summary>
internal sealed partial class QueryVisitor : IVisitor
{
    /// <inheritdoc />
    public void Visit(SelectComponent element)
    {
        SqlBuilder.Append(element.SqlBuilder);
    }

    /// <inheritdoc />
    public void Visit(SelectDistinctComponent element)
    {
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
