namespace KISS.FluentSqlBuilder.Utils;

/// <summary>
/// Defines the various aggregation types used in SQL queries for calculating summary statistics.
/// </summary>
public enum SqlAggregation
{
    /// <summary>
    /// Represents the SQL SUM() function, which calculates the total sum of a column.
    /// </summary>
    Sum,

    /// <summary>
    /// Represents the SQL AVG() function, which calculates the average of a column.
    /// </summary>
    Avg,

    /// <summary>
    /// Represents the SQL MIN() function, which returns the minimum value from a column.
    /// </summary>
    Min,

    /// <summary>
    /// Represents the SQL MAX() function, which returns the maximum value from a column.
    /// </summary>
    Max,

    /// <summary>
    /// Represents the SQL COUNT() function,  which returns the number of items found in a group.
    /// </summary>
    Count
}
