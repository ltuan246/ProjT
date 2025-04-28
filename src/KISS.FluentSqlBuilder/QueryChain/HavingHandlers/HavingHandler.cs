namespace KISS.FluentSqlBuilder.QueryChain.HavingHandlers;

/// <summary>
///     A handler for processing HAVING clauses in a query chain.
///     This class is responsible for generating SQL HAVING statements that filter
///     grouped results based on aggregate conditions.
/// </summary>
/// <param name="Predicate">
///     An expression defining the aggregate conditions to filter the grouped results.
///     This expression typically contains aggregate functions and comparison operators.
/// </param>
public sealed partial record HavingHandler(Expression Predicate) : QueryHandler(SqlStatement.Having, Predicate);