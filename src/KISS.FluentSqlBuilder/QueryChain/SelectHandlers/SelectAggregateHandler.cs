namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     A handler for processing aggregate SELECT clauses in a query chain.
///     This class is responsible for generating SQL statements for aggregate functions
///     like COUNT, SUM, AVG, etc., and assigning aliases to the results.
/// </summary>
/// <param name="Selector">
///     An expression defining the aggregation operation to be performed in the SELECT clause.
///     This can be any valid aggregate function expression (e.g., COUNT, SUM, AVG).
/// </param>
/// <param name="Alias">
///     The alias to assign to the aggregate result in the SQL query.
///     This name will be used to reference the result in the query output.
/// </param>
public sealed partial record SelectAggregateHandler(Expression Selector, string Alias)
    : QueryHandler(SqlStatement.SelectAggregate, Selector);
