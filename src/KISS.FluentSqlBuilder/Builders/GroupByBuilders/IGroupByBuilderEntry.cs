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
