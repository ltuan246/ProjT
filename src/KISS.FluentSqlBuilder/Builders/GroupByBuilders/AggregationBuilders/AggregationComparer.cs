namespace KISS.FluentSqlBuilder.Builders.GroupByBuilders.AggregationBuilders;

/// <summary>
///     Represents an aggregation operation result that supports comparison operators.
///     This class encapsulates the aggregation type (e.g., "Sum", "Max") and selector expression,
///     enabling conditions to be applied in a IHavingBuilder clause.
/// </summary>
/// <typeparam name="T">The type of the data being queried and aggregated.</typeparam>
/// <typeparam name="TValueType">The type of the value produced by the aggregation, which must support comparison.</typeparam>
internal sealed record AggregationComparer<T, TValueType>
    where TValueType : IComparable<TValueType>;
