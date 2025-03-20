namespace KISS.FluentSqlBuilder.Utils;

/// <summary>
///     An enumeration representing different types of SQL statements.
///     Used to identify and validate the order of SQL clauses in the query building process.
/// </summary>
public enum SqlStatement
{
    /// <summary>
    ///     Represents a <c>SELECT</c> clause in a SQL query.
    /// </summary>
    Select,

    /// <summary>
    ///     Represents a <c>FROM</c> clause in a SQL query.
    /// </summary>
    From,

    /// <summary>
    ///     Represents a <c>JOIN</c> clause in a SQL query.
    /// </summary>
    Join,

    /// <summary>
    ///     Represents a <c>WHERE</c> clause in a SQL query.
    /// </summary>
    Where,

    /// <summary>
    ///     Represents a <c>GROUP BY</c> clause in a SQL query.
    /// </summary>
    GroupBy,

    /// <summary>
    ///     Represents a <c>HAVING</c> clause in a SQL query.
    /// </summary>
    Having,

    /// <summary>
    ///     Represents a <c>SELECT</c> clause with aggregations in a SQL query.
    /// </summary>
    SelectAggregate,

    /// <summary>
    ///     Represents an <c>ORDER BY</c> clause in a SQL query.
    /// </summary>
    OrderBy,

    /// <summary>
    ///     Represents a <c>LIMIT</c> clause in a SQL query.
    /// </summary>
    Limit,

    /// <summary>
    ///     Represents an <c>OFFSET</c> clause in a SQL query.
    /// </summary>
    Offset
}
