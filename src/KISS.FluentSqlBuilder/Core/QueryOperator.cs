namespace KISS.FluentSqlBuilder.Core;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">
///     The type of objects to return, representing the query result rows.
///     This type must match the structure of the query results.
/// </typeparam>
public sealed partial record QueryOperator<TRecordset, TReturn>(QueryHandler ChainHandler, IComposite Composite, List<IDictionary<string, object>> InputData) : ICompositeQueryOperations<TRecordset, TReturn>;
