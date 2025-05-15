namespace KISS.FluentSqlBuilder.Decorators.SelectDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TIn">The type representing the database record set.</typeparam>
/// <typeparam name="TOut">The combined type to return.</typeparam>
public sealed partial record SelectDecorator<TIn, TOut>
{
    /// <summary>
    ///     Gets the StringBuilder instance used to construct the SQL query.
    ///     This builder accumulates SQL statements and clauses during query construction.
    /// </summary>
    public StringBuilder SqlBuilder { get; } = new();

    /// <summary>
    ///     Gets the SQL formatter instance used for custom string formatting
    ///     and parameter handling in SQL queries.
    /// </summary>
    public SqlFormatter SqlFormatting { get; init; }
}