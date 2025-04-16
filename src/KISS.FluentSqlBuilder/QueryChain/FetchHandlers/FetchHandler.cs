namespace KISS.FluentSqlBuilder.QueryChain.FetchHandlers;

/// <summary>
///     A handler for processing FETCH clauses in a query chain.
///     This class is responsible for generating SQL FETCH statements that specify
///     the number of rows to retrieve after skipping a specified offset.
/// </summary>
/// <param name="Offset">
///     An expression specifying the number of rows to fetch.
///     This expression can be a constant or a dynamic value.
/// </param>
// public sealed record FetchHandler(Expression Offset) : QueryHandler
// {
//     /// <summary>
//     ///     Processes the FETCH clause by adding the fetch expression
//     ///     to the query's FETCH statements.
//     /// </summary>
//     protected override void Process() { }
// }
