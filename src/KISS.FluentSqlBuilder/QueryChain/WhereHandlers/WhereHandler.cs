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
public sealed partial record WhereHandler(Expression Predicate) : QueryHandler(SqlStatement.Where, Predicate)
{
    /// <inheritdoc />
    protected override void Process()
    {
        // Ensures the composite query is wrapped with a WhereDecorator for WHERE clause processing.
        if (Composite is not WhereDecorator)
        {
            Composite = new WhereDecorator(Composite);
        }
    }
}
