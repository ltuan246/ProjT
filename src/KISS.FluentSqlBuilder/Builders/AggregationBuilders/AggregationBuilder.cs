namespace KISS.FluentSqlBuilder.Builders.AggregationBuilders;

/// <summary>
///     Provides a builder for defining SQL aggregation operations in queries. This class
///     enables the construction of complex aggregation expressions for use in GROUP BY
///     queries and HAVING clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view being queried.</typeparam>
public sealed record AggregationBuilder<TRecordset>
{
    /// <summary>
    ///     Defines a SUM aggregation operation on a selected property.
    ///     This method calculates the total sum of values for the specified property.
    /// </summary>
    /// <param name="selector">An expression selecting the numeric property to sum.</param>
    /// <returns>
    ///     An <see cref="AggregationComparer{TRecordset}"/> representing the SUM operation,
    ///     which can be used in HAVING clauses for filtering aggregated results.
    /// </returns>
    public AggregationComparer<TRecordset> Sum(Expression<Func<TRecordset, IComparable?>> selector)
        => new(SqlAggregation.Sum, selector);

    /// <summary>
    ///     Defines a MAX aggregation operation on a selected property.
    ///     This method finds the maximum value for the specified property.
    /// </summary>
    /// <param name="selector">An expression selecting the property to find the maximum value of.</param>
    /// <returns>
    ///     An <see cref="AggregationComparer{TRecordset}"/> representing the MAX operation,
    ///     which can be used in HAVING clauses for filtering aggregated results.
    /// </returns>
    public AggregationComparer<TRecordset> Max(Expression<Func<TRecordset, IComparable?>> selector)
        => new(SqlAggregation.Max, selector);

    /// <summary>
    ///     Defines a MIN aggregation operation on a selected property.
    ///     This method finds the minimum value for the specified property.
    /// </summary>
    /// <param name="selector">An expression selecting the property to find the minimum value of.</param>
    /// <returns>
    ///     An <see cref="AggregationComparer{TRecordset}"/> representing the MIN operation,
    ///     which can be used in HAVING clauses for filtering aggregated results.
    /// </returns>
    public AggregationComparer<TRecordset> Min(Expression<Func<TRecordset, IComparable?>> selector)
        => new(SqlAggregation.Min, selector);

    /// <summary>
    ///     Defines an AVG aggregation operation on a selected property.
    ///     This method calculates the average value for the specified property.
    /// </summary>
    /// <param name="selector">An expression selecting the numeric property to average.</param>
    /// <returns>
    ///     An <see cref="AggregationComparer{TRecordset}"/> representing the AVG operation,
    ///     which can be used in HAVING clauses for filtering aggregated results.
    /// </returns>
    public AggregationComparer<TRecordset> Avg(Expression<Func<TRecordset, IComparable?>> selector)
        => new(SqlAggregation.Avg, selector);

    /// <summary>
    ///     Defines a COUNT aggregation operation on a selected property.
    ///     This method counts the number of rows for the specified property.
    /// </summary>
    /// <param name="selector">An expression selecting the property to count.</param>
    /// <returns>
    ///     An <see cref="AggregationComparer{TRecordset}"/> representing the COUNT operation,
    ///     which can be used in HAVING clauses for filtering aggregated results.
    /// </returns>
    public AggregationComparer<TRecordset> Count(Expression<Func<TRecordset, IComparable?>> selector)
        => new(SqlAggregation.Count, selector);
}
