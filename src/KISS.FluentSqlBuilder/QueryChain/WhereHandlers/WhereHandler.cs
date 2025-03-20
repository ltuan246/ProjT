namespace KISS.FluentSqlBuilder.QueryChain.WhereHandlers;

/// <summary>
///     A handler for processing WHERE clauses in a query chain.
///     This class is responsible for translating LINQ expressions into SQL WHERE conditions
///     and adding them to the query being built.
/// </summary>
/// <param name="Predicate">
///     The LINQ expression that defines the WHERE conditions.
///     This expression will be translated into SQL-compatible form.
/// </param>
public sealed partial record WhereHandler(Expression Predicate) : QueryHandler
{
    /// <summary>
    ///     Processes the WHERE clause by translating the predicate expression
    ///     into SQL and adding it to the query is WHERE statements.
    /// </summary>
    protected override void Process()
    {
        Translate(Predicate);
        Composite.SqlStatements[SqlStatement.Where].Add($"{StatementBuilder}");
    }
}
