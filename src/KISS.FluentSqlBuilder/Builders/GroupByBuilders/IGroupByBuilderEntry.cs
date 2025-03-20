namespace KISS.FluentSqlBuilder.Builders.GroupByBuilders;

/// <summary>
///     An interface for adding a <c>GROUP BY</c> clause to the query. This interface
///     provides type-safe methods for grouping query results by specified properties.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view being queried.</typeparam>
/// <typeparam name="TReturn">The type of the combined result set after grouping.</typeparam>
public interface IGroupByBuilderEntry<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause to the query, grouping results by the
    ///     specified property.
    /// </summary>
    /// <param name="selector">An expression selecting the property to group by.</param>
    /// <returns>
    ///     An <see cref="IGroupByBuilder{TRecordset, TReturn}" /> instance that can be used
    ///     to add aggregations or HAVING clauses to the grouped query.
    /// </returns>
    IGroupByBuilder<TRecordset, TReturn> GroupBy(Expression<Func<TRecordset, IComparable>> selector);
}

/// <summary>
///     An interface for adding a <c>GROUP BY</c> clause to a query involving two tables.
///     This interface extends the grouping functionality to support queries with joins.
/// </summary>
/// <typeparam name="TFirst">The type representing the first table in the join.</typeparam>
/// <typeparam name="TSecond">The type representing the second table in the join.</typeparam>
/// <typeparam name="TReturn">The type of the combined result set after grouping.</typeparam>
public interface IGroupByBuilderEntry<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause to the query, grouping results by a
    ///     property from the first table.
    /// </summary>
    /// <param name="selector">An expression selecting the property from the first table to group by.</param>
    /// <returns>
    ///     An <see cref="IGroupByBuilder{TFirst, TReturn}" /> instance that can be used
    ///     to add aggregations or HAVING clauses to the grouped query.
    /// </returns>
    IGroupByBuilder<TFirst, TReturn> GroupBy(Expression<Func<TFirst, IComparable>> selector);
}

/// <summary>
///     An interface for adding a <c>GROUP BY</c> clause to a query involving three tables.
///     This interface extends the grouping functionality to support complex queries with multiple joins.
/// </summary>
/// <typeparam name="TFirst">The type representing the first table in the join.</typeparam>
/// <typeparam name="TSecond">The type representing the second table in the join.</typeparam>
/// <typeparam name="TThird">The type representing the third table in the join.</typeparam>
/// <typeparam name="TReturn">The type of the combined result set after grouping.</typeparam>
public interface IGroupByBuilderEntry<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause to the query, grouping results by a
    ///     property from the first table.
    /// </summary>
    /// <param name="selector">An expression selecting the property from the first table to group by.</param>
    /// <returns>
    ///     An <see cref="IGroupByBuilder{TFirst, TReturn}" /> instance that can be used
    ///     to add aggregations or HAVING clauses to the grouped query.
    /// </returns>
    IGroupByBuilder<TFirst, TReturn> GroupBy(Expression<Func<TFirst, IComparable?>> selector);
}
