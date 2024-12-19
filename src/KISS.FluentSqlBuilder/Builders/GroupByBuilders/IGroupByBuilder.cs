namespace KISS.FluentSqlBuilder.Builders.GroupByBuilders;

/// <summary>
///     An interface for building <c>GROUP BY</c> clauses.
/// </summary>
public interface IGroupByBuilder;

/// <summary>
///     An interface for building <c>GROUP BY</c> clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IGroupByBuilder<TRecordset, TReturn> :
    IHavingBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Specifies an additional grouping key to refine the grouped data
    ///     after the initial <c>GROUP BY</c> clause in the query.
    /// </summary>
    /// <param name="selector">An additional key to further refine the grouping.</param>
    /// <returns>The <see cref="IGroupByBuilder{TRecordset, TReturn}" /> instance.</returns>
    IGroupByBuilder<TRecordset, TReturn> ThenBy(Expression<Func<TRecordset, IComparable>> selector);
}
