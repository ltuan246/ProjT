namespace KISS.FluentSqlBuilder.QueryChain.Handlers;

/// <summary>
///     OrderByHandler.
/// </summary>
/// <param name="Selector">Selector.</param>
public sealed partial record OrderByHandler(Expression Selector) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
    {
        Translate(Selector);
        Composite.SqlStatements[SqlStatement.OrderBy].Add($"{StatementBuilder.ToString()}");
    }
}
