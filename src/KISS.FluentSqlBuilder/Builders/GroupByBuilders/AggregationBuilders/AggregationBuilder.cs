namespace KISS.FluentSqlBuilder.Builders.GroupByBuilders.AggregationBuilders;

/// <summary>
///     Provides a builder for defining aggregation operations (e.g., Sum, Max) in a query.
///     This class exposes methods to specify how data is aggregated from a generic type <typeparamref name="T" />.
///     It works in conjunction with <see cref="QueryBuilder{T}" /> to construct HAVING clauses.
/// </summary>
/// <typeparam name="T">The type of the data being queried and aggregated.</typeparam>
public sealed record AggregationBuilder<T>;
