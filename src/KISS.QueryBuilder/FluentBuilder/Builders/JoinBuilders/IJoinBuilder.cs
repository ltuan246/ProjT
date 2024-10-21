namespace KISS.QueryBuilder.FluentBuilder.Builders.JoinBuilders;

/// <summary>
///     An interface that defines the join builder type.
/// </summary>
/// <typeparam name="TRecordset">The type in the recordset.</typeparam>
public interface IJoinBuilder<TRecordset> : IWhereBuilder
{
    /// <summary>
    ///     Appends the <c>INNER JOIN</c> clause to the builder.
    /// </summary>
    /// <param name="leftKeySelector">The table as the left key.</param>
    /// <param name="rightKeySelector">The table as the right key.</param>
    /// <typeparam name="TRelation">The type of table that want to join.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TRecordset}" /> instance.</returns>
    IJoinBuilder<TRecordset> Join<TRelation>(
        Expression<Func<TRecordset, object>> leftKeySelector,
        Expression<Func<TRelation, object>> rightKeySelector);

    /// <summary>
    ///     Appends the <c>INNER JOIN</c> clause to the builder.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <param name="leftKeySelector">The table as the left key.</param>
    /// <param name="rightKeySelector">The table as the right key.</param>
    /// <typeparam name="TRelation">The type of table that want to join.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TRecordset}" /> instance.</returns>
    IJoinBuilder<TRecordset> Join<TRelation>(
        bool condition,
        Expression<Func<TRecordset, object>> leftKeySelector,
        Expression<Func<TRelation, object>> rightKeySelector);
}
