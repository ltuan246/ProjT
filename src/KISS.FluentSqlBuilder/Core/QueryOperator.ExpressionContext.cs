namespace KISS.FluentSqlBuilder.Core;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">
///     The type of objects to return, representing the query result rows.
///     This type must match the structure of the query results.
/// </typeparam>
public sealed partial record QueryOperator<TRecordset, TReturn>;