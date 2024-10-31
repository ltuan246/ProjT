namespace KISS.QueryBuilder.Builders.HavingBuilders;

/// <summary>
///     An interface for building SQL HAVING clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public interface IHavingBuilder<TRecordset> : IOrderByBuilderEntry<TRecordset>
{
    /// <summary>
    ///     Appends the <c>HAVING</c> clause to the query.
    /// </summary>
    /// <returns>The <see cref="IHavingBuilder{TRecordset}" /> instance.</returns>
    IHavingBuilder<TRecordset> Having();

    /// <summary>
    ///     Appends the <c>HAVING</c> clause with a condition.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IHavingBuilder{TRecordset}" /> instance.</returns>
    IHavingBuilder<TRecordset> Having(bool condition);
}
