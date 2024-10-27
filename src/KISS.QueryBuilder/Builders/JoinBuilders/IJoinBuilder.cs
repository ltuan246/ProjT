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
    /// <param name="mapSelector">The results of the joined tables to be mapped back into the selector.</param>
    /// <param name="leftKeySelector">The table as the left key.</param>
    /// <param name="rightKeySelector">The table as the right key.</param>
    /// <typeparam name="TRelation">The type of table that want to join.</typeparam>
    /// <typeparam name="TKey">The compare keys.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TRecordset}" /> instance.</returns>
    IJoinBuilder<TRecordset> InnerJoin<TRelation, TKey>(
        Expression<Func<TRecordset, TRelation>> mapSelector,
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>;

    /// <summary>
    ///     Appends the <c>JOIN</c> clause with a condition.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <returns>The <see cref="IJoinBuilder{TRecordset}" /> instance.</returns>
    IJoinBuilder<TRecordset> InnerJoin(bool condition);
}
