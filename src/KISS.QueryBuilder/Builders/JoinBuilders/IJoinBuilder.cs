namespace KISS.QueryBuilder.Builders.JoinBuilders;

/// <summary>
///     An interface for building SQL join conditions.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface IJoinBuilder<TRecordset> : IWhereBuilder<TRecordset>
{
    /// <summary>
    ///     Appends the <c>JOIN</c> clause to the query.
    /// </summary>
    /// <returns>The <see cref="IJoinBuilder{TRecordset}" /> instance.</returns>
    IJoinBuilder<TRecordset> InnerJoin();

    /// <summary>
    ///     Appends the <c>JOIN</c> clause with a condition.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IJoinBuilder{TRecordset}" /> instance.</returns>
    IJoinBuilder<TRecordset> InnerJoin(bool condition);
}
