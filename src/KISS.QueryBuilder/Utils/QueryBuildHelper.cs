namespace KISS.QueryBuilder.Utils;

internal static class QueryBuildHelper
{
    public static IReadOnlyDictionary<ComparisonOperator, string> FieldMatchingOperators { get; } =
        new Dictionary<ComparisonOperator, string>
        {
            [ComparisonOperator.Equals] = " = ",
            [ComparisonOperator.NotEquals] = " <> ",
            [ComparisonOperator.Greater] = " > ",
            [ComparisonOperator.GreaterOrEquals] = " >= ",
            [ComparisonOperator.Less] = " < ",
            [ComparisonOperator.LessOrEquals] = " <= "
        };

    public static IReadOnlyDictionary<SingleItemAsArrayOperator, string> SingleItemAsArrayOperators { get; } =
        new Dictionary<SingleItemAsArrayOperator, string>
        {
            [SingleItemAsArrayOperator.Contains] = " IN ", [SingleItemAsArrayOperator.NotContains] = " NOT IN "
        };

    public static IReadOnlyDictionary<LogicalOperator, string> LogicalOperators { get; } =
        new Dictionary<LogicalOperator, string> { [LogicalOperator.And] = " AND ", [LogicalOperator.Or] = " OR " };

    public static IReadOnlyDictionary<SortDirection, string> OrderByOperators { get; } =
        new Dictionary<SortDirection, string>
        {
            [SortDirection.Ascending] = " ASC ", [SortDirection.Descending] = " DESC "
        };
}