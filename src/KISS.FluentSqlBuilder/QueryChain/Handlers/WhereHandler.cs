namespace KISS.FluentSqlBuilder.QueryChain.Handlers;

/// <summary>
///     WhereHandler.
/// </summary>
/// <param name="Predicate">Predicate.</param>
public sealed partial record WhereHandler(Expression Predicate) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
    {
        Translate(Predicate);
        Composite.SqlStatements[SqlStatement.Where].Add($"{StatementBuilder.ToString()}");
    }
}
