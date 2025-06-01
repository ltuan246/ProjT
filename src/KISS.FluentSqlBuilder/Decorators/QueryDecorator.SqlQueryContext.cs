namespace KISS.FluentSqlBuilder.Decorators;

/// <summary>
///     Provides the SQL query context and core properties for the <see cref="QueryDecorator"/>.
///     This abstract base exposes the SQL string, parameters, statement collections, and builder utilities
///     required for constructing and executing composite SQL queries with type-safe result processing.
/// </summary>
public abstract partial record QueryDecorator
{
    /// <inheritdoc />
    public abstract string Sql { get; }

    /// <inheritdoc />
    public DynamicParameters Parameters
        => SqlFormatting.Parameters;

    /// <inheritdoc />
    public Dictionary<SqlStatement, List<string>> SqlStatements { get; } = Inner.SqlStatements;

    /// <summary>
    ///     Gets the <see cref="StringBuilder"/> instance used to construct the SQL query.
    ///     This builder accumulates SQL statements and clauses during query construction.
    /// </summary>
    public StringBuilder SqlBuilder { get; } = new();

    /// <summary>
    ///     Gets the <see cref="SqlFormatter"/> instance used for custom string formatting
    ///     and parameter handling in SQL queries.
    /// </summary>
    public SqlFormatter SqlFormatting { get; } = Inner.SqlFormatting;
}
