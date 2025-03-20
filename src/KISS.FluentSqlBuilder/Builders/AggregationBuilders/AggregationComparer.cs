namespace KISS.FluentSqlBuilder.Builders.AggregationBuilders;

/// <summary>
///     Represents an aggregation operation result that supports comparison operators.
///     This class enables the construction of HAVING clauses by providing comparison
///     operators for aggregated values.
/// </summary>
/// <param name="AggregationType">The type of aggregation operation to perform (e.g., SUM, MAX, MIN).</param>
/// <param name="Selector">An expression selecting the property to aggregate.</param>
/// <typeparam name="TRecordset">The type representing the database table or view being queried.</typeparam>
public sealed record AggregationComparer<TRecordset>(
    SqlAggregation AggregationType,
    Expression<Func<TRecordset, IComparable?>> Selector)
{
    /// <summary>
    ///     Implements the greater than operator for comparing aggregated values.
    ///     This operator is used in HAVING clauses to filter groups where the
    ///     aggregated value is greater than the specified value.
    /// </summary>
    /// <param name="left">The aggregation comparer representing the aggregated value.</param>
    /// <param name="right">The value to compare against.</param>
    /// <returns>True if the aggregated value is greater than the specified value.</returns>
    public static bool operator >(AggregationComparer<TRecordset> left, IComparable right)
    {
        _ = left;
        _ = right;
        return true;
    }

    /// <summary>
    ///     Implements the less than operator for comparing aggregated values.
    ///     This operator is used in HAVING clauses to filter groups where the
    ///     aggregated value is less than the specified value.
    /// </summary>
    /// <param name="left">The aggregation comparer representing the aggregated value.</param>
    /// <param name="right">The value to compare against.</param>
    /// <returns>True if the aggregated value is less than the specified value.</returns>
    public static bool operator <(AggregationComparer<TRecordset> left, IComparable right)
    {
        _ = left;
        _ = right;
        return true;
    }

    /// <summary>
    ///     Implements the equality operator for comparing aggregated values.
    ///     This operator is used in HAVING clauses to filter groups where the
    ///     aggregated value equals the specified value.
    /// </summary>
    /// <param name="left">The aggregation comparer representing the aggregated value.</param>
    /// <param name="right">The value to compare against.</param>
    /// <returns>True if the aggregated value equals the specified value.</returns>
    public static bool operator ==(AggregationComparer<TRecordset> left, IComparable right)
    {
        _ = left;
        _ = right;
        return true;
    }

    /// <summary>
    ///     Implements the inequality operator for comparing aggregated values.
    ///     This operator is used in HAVING clauses to filter groups where the
    ///     aggregated value does not equal the specified value.
    /// </summary>
    /// <param name="left">The aggregation comparer representing the aggregated value.</param>
    /// <param name="right">The value to compare against.</param>
    /// <returns>True if the aggregated value does not equal the specified value.</returns>
    public static bool operator !=(AggregationComparer<TRecordset> left, IComparable right)
    {
        _ = left;
        _ = right;
        return true;
    }

    /// <summary>
    ///     Implements the less than or equal operator for comparing aggregated values.
    ///     This operator is used in HAVING clauses to filter groups where the
    ///     aggregated value is less than or equal to the specified value.
    /// </summary>
    /// <param name="left">The aggregation comparer representing the aggregated value.</param>
    /// <param name="right">The value to compare against.</param>
    /// <returns>True if the aggregated value is less than or equal to the specified value.</returns>
    public static bool operator <=(AggregationComparer<TRecordset> left, IComparable right)
    {
        _ = left;
        _ = right;
        return true;
    }

    /// <summary>
    ///     Implements the greater than or equal operator for comparing aggregated values.
    ///     This operator is used in HAVING clauses to filter groups where the
    ///     aggregated value is greater than or equal to the specified value.
    /// </summary>
    /// <param name="left">The aggregation comparer representing the aggregated value.</param>
    /// <param name="right">The value to compare against.</param>
    /// <returns>True if the aggregated value is greater than or equal to the specified value.</returns>
    public static bool operator >=(AggregationComparer<TRecordset> left, IComparable right)
    {
        _ = left;
        _ = right;
        return true;
    }
}
