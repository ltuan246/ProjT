namespace KISS.QueryBuilder.Builders.GroupByBuilders;

/// <summary>
///     An interface for adding a GROUP BY clause to the query.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface IGroupByBuilderEntry<TRecordset> : IFluentSqlBuilder<TRecordset>
{
    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause to the query.
    /// </summary>
    /// <returns>The <see cref="IGroupByBuilder{TRecordset}" /> instance.</returns>
    IGroupByBuilder<TRecordset> GroupBy();

    /// <summary>
    ///     Appends the <c>GROUP BY</c> clause with a condition.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IGroupByBuilder{TRecordset}" /> instance.</returns>
    IGroupByBuilder<TRecordset> GroupBy(bool condition);
}
