namespace KISS.FluentSqlBuilder.Utils;

/// <summary>
///     Defines the various aggregation types that can be used in SQL queries.
///     These aggregation functions are used to calculate summary statistics
///     from groups of rows in a query result.
/// </summary>
public enum SqlAggregation
{
    /// <summary>
    ///     Represents the SQL SUM() function for calculating the total sum of a column.
    /// </summary>
    Sum,

    /// <summary>
    ///     Represents the SQL AVG() function for calculating the average of a column.
    /// </summary>
    Avg,

    /// <summary>
    ///     Represents the SQL MIN() function for returning the minimum value from a column.
    /// </summary>
    Min,

    /// <summary>
    ///     Represents the SQL MAX() function for returning the maximum value from a column.
    /// </summary>
    Max,

    /// <summary>
    ///     Represents the SQL COUNT() function for returning the number of items found in a group.
    /// </summary>
    Count
}
