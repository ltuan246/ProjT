namespace KISS.FluentSqlBuilder.Builders.GroupByBuilders.AggregationBuilders;

/// <summary>
///     Represents an aggregation operation result that supports comparison operators.
///     This class encapsulates the aggregation type (e.g., "Sum", "Max") and selector expression,
///     enabling conditions to be applied in a IHavingBuilder clause.
/// </summary>
/// <typeparam name="TRecordset">The type of the data being queried and aggregated.</typeparam>
public sealed record AggregationComparer<TRecordset>(SqlAggregation AggregationType, Expression<Func<TRecordset, IComparable?>> Selector)
{
    public static bool operator >(AggregationComparer<TRecordset> left, IComparable right)
    {
        _ = left;
        _ = right;
        return true;
    }

    public static bool operator <(AggregationComparer<TRecordset> left, IComparable right)
    {
        _ = left;
        _ = right;
        return true;
    }

    public static bool operator ==(AggregationComparer<TRecordset> left, IComparable right)
    {
        _ = left;
        _ = right;
        return true;
    }

    public static bool operator !=(AggregationComparer<TRecordset> left, IComparable right)
    {
        _ = left;
        _ = right;
        return true;
    }

    public static bool operator <=(AggregationComparer<TRecordset> left, IComparable right)
    {
        _ = left;
        _ = right;
        return true;
    }

    public static bool operator >=(AggregationComparer<TRecordset> left, IComparable right)
    {
        _ = left;
        _ = right;
        return true;
    }
}
