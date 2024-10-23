namespace KISS.QueryBuilder.FluentBuilder.Builders.JoinBuilders;

/// <summary>
///     An interface that defines the join builder type.
/// </summary>
/// <typeparam name="TRecordset">The type in the recordset.</typeparam>
public interface IJoinBuilder<TRecordset> : IWhereBuilder<TRecordset>
{
    /// <summary>
    ///     Appends the <c>INNER JOIN</c> clause to the builder.
    /// </summary>
    /// <param name="resultSelector">The results of the joined tables to be mapped back into the key.</param>
    /// <param name="leftKeySelector">The table as the left key.</param>
    /// <param name="rightKeySelector">The table as the right key.</param>
    /// <typeparam name="TRelation">The type of table that want to join.</typeparam>
    /// <typeparam name="TKey">The compare keys.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TRecordset}" /> instance.</returns>
    IJoinBuilder<TRecordset> Join<TRelation, TKey>(
        Expression<Func<TRecordset, TRelation>> resultSelector,
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector);

    /// <summary>
    ///     Appends the <c>INNER JOIN</c> clause to the builder.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <param name="resultSelector">The results of the joined tables to be mapped back into the key.</param>
    /// <param name="leftKeySelector">The table as the left key.</param>
    /// <param name="rightKeySelector">The table as the right key.</param>
    /// <typeparam name="TRelation">The type of table that want to join.</typeparam>
    /// <typeparam name="TKey">The compare keys.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TRecordset}" /> instance.</returns>
    IJoinBuilder<TRecordset> Join<TRelation, TKey>(
        bool condition,
        Expression<Func<TRecordset, TRelation>> resultSelector,
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector);
}
