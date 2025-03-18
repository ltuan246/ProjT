namespace KISS.FluentSqlBuilder.QueryChain.WhereHandlers;

/// <summary>
///     A handler for processing <c>WHERE</c> in a query chain.
/// </summary>
/// <param name="Predicate">Filters a sequence of values based on a predicate.</param>
public sealed partial record WhereHandler(Expression Predicate) : QueryHandler
{
    /// <inheritdoc />
    protected override void Process()
    {
        Translate(Predicate);
        Composite.SqlStatements[SqlStatement.Where].Add($"{StatementBuilder}");
    }
}
