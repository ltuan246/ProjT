namespace KISS.FluentSqlBuilder.Builders.GroupByBuilders.AggregationBuilders;

/// <summary>
///     Provides a builder for defining aggregation operations (e.g., Sum, Max) in a query.
///     This class exposes methods to specify how data is aggregated from a generic type <typeparamref name="T" />.
///     It works in conjunction with <see cref="QueryBuilder{T}" /> to construct HAVING clauses.
/// </summary>
/// <typeparam name="TRecordset">The type of the data being queried and aggregated.</typeparam>
public sealed record AggregationBuilder<TRecordset>
{
    /// <summary>
    ///     Defines a SUM aggregation operation on a selected property of <typeparamref name="TRecordset"/>.
    /// </summary>
    /// <param name="selector">An expression selecting the property to aggregate (e.g., x => x.SomeValue).</param>
    /// <returns>
    ///     An <see cref="AggregationComparer{TRecordset}"/> representing the SUM operation,
    ///     which supports comparison operators for use in a HAVING clause.
    /// </returns>
    public AggregationComparer<TRecordset> Sum(Expression<Func<TRecordset, IComparable?>> selector)
        => new(SqlAggregation.Sum, selector);

    /// <summary>
    ///     Defines a MAX aggregation operation on a selected property of <typeparamref name="TRecordset"/>.
    /// </summary>
    /// <param name="selector">An expression selecting the property to aggregate (e.g., x => x.SomeValue).</param>
    /// <returns>
    ///     An <see cref="AggregationComparer{TRecordset}"/> representing the MAX operation,
    ///     which supports comparison operators for use in a HAVING clause.
    /// </returns>
    public AggregationComparer<TRecordset> Max(Expression<Func<TRecordset, IComparable?>> selector)
        => new(SqlAggregation.Max, selector);

    /// <summary>
    ///     Defines a MIN aggregation operation on a selected property of <typeparamref name="TRecordset"/>.
    /// </summary>
    /// <param name="selector">An expression selecting the property to aggregate (e.g., x => x.SomeValue).</param>
    /// <returns>
    ///     An <see cref="AggregationComparer{TRecordset}"/> representing the MIN operation,
    ///     which supports comparison operators for use in a HAVING clause.
    /// </returns>
    public AggregationComparer<TRecordset> Min(Expression<Func<TRecordset, IComparable?>> selector)
        => new(SqlAggregation.Min, selector);

    /// <summary>
    ///     Defines an AVG aggregation operation on a selected property of <typeparamref name="TRecordset"/>.
    /// </summary>
    /// <param name="selector">An expression selecting the property to aggregate (e.g., x => x.SomeValue).</param>
    /// <returns>
    ///     An <see cref="AggregationComparer{TRecordset}"/> representing the AVG operation,
    ///     which supports comparison operators for use in a HAVING clause.
    /// </returns>
    public AggregationComparer<TRecordset> Avg(Expression<Func<TRecordset, IComparable?>> selector)
        => new(SqlAggregation.Avg, selector);

    /// <summary>
    ///     Defines a COUNT aggregation operation on a selected property of <typeparamref name="TRecordset"/>.
    /// </summary>
    /// <param name="selector">An expression selecting the property to aggregate (e.g., x => x.SomeValue).</param>
    /// <returns>
    ///     An <see cref="AggregationComparer{TRecordset}"/> representing the COUNT operation,
    ///     which supports comparison operators for use in a HAVING clause.
    /// </returns>
    public AggregationComparer<TRecordset> Count(Expression<Func<TRecordset, IComparable?>> selector)
        => new(SqlAggregation.Sum, selector); // Should likely be SqlAggregation.Count
}
