namespace KISS.FluentSqlBuilder.Builders.GroupByBuilders;

/// <summary>
///     An interface for adding a <c>GROUP BY</c> clause to the query.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IGroupByBuilderEntry<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause to the query.
    /// </summary>
    /// <param name="selector">The grouping key.</param>
    /// <returns>The <see cref="IGroupByBuilder{TRecordset, TReturn}" /> instance.</returns>
    IGroupByBuilder<TRecordset, TReturn> GroupBy(Expression<Func<TRecordset, IComparable>> selector);
}

/// <summary>
///     An interface for adding a <c>GROUP BY</c> clause to the query.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IGroupByBuilderEntry<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause to the query.
    /// </summary>
    /// <param name="selector">The grouping key.</param>
    /// <returns>The <see cref="IGroupByBuilder{TRecordset, TReturn}" /> instance.</returns>
    IGroupByBuilder<TFirst, TReturn> GroupBy(Expression<Func<TFirst, IComparable>> selector);
}

/// <summary>
///     An interface for adding a <c>GROUP BY</c> clause to the query.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IGroupByBuilderEntry<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause to the query.
    /// </summary>
    /// <param name="selector">The grouping key.</param>
    /// <returns>The <see cref="IGroupByBuilder{TRecordset, TReturn}" /> instance.</returns>
    IGroupByBuilder<TFirst, TReturn> GroupBy(Expression<Func<TFirst, IComparable?>> selector);
}
