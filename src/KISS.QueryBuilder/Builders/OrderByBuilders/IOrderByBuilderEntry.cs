namespace KISS.QueryBuilder.Builders.OrderByBuilders;

/// <summary>
///     An interface for adding an ORDER BY clause to the query.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface IOrderByBuilderEntry<TRecordset> : IFluentSqlBuilder<TRecordset>
{
    /// <summary>
    ///     Appends the <c>ORDER BY</c> clause to the query.
    /// </summary>
    /// <param name="selector">The table columns.</param>
    /// <returns>The <see cref="IOrderByBuilder{TRecordset}" /> instance.</returns>
    IOrderByBuilder<TRecordset> OrderBy(Expression<Func<TRecordset, object>> selector);

    /// <summary>
    ///     Appends the <c>ORDER BY</c> clause with a condition.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IOrderByBuilder{TRecordset}" /> instance.</returns>
    IOrderByBuilder<TRecordset> OrderBy(bool condition);
}
