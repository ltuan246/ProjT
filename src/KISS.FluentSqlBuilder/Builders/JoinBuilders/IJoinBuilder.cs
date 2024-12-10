namespace KISS.FluentSqlBuilder.Builders.JoinBuilders;

/// <summary>
///     An interface for adding a <c>JOIN</c> clause to the query.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IJoinBuilder<TRecordset, TReturn> : IWhereBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>JOIN</c> clause to the query.
    /// </summary>
    /// <param name="leftKeySelector">The table as the left key.</param>
    /// <param name="rightKeySelector">The table as the right key.</param>
    /// <typeparam name="TRelation">The type of join table.</typeparam>
    /// <typeparam name="TKey">The compare keys.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TFirst, TSecond, TReturn}" /> instance.</returns>
    IJoinBuilder<TRecordset, TRelation, TReturn> InnerJoin<TRelation, TKey>(
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>;
}

/// <summary>
///     An interface for adding a <c>JOIN</c> clause to the query.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IJoinBuilder<TFirst, TSecond, TReturn> : IWhereBuilder<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends the <c>JOIN</c> clause to the query.
    /// </summary>
    /// <param name="leftKeySelector">The table as the left key.</param>
    /// <param name="rightKeySelector">The table as the right key.</param>
    /// <typeparam name="TRelation">The type of join table.</typeparam>
    /// <typeparam name="TKey">The compare keys.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TFirst, TSecond, TThird, TReturn}" /> instance.</returns>
    IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation, TKey>(
        Expression<Func<TFirst, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>;

    /// <summary>
    ///     Appends the <c>JOIN</c> clause to the query.
    /// </summary>
    /// <param name="leftKeySelector">The table as the left key.</param>
    /// <param name="rightKeySelector">The table as the right key.</param>
    /// <typeparam name="TRelation">The type of join table.</typeparam>
    /// <typeparam name="TKey">The compare keys.</typeparam>
    /// <returns>The <see cref="IJoinBuilder{TFirst, TSecond, TThird, TReturn}" /> instance.</returns>
    IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation, TKey>(
        Expression<Func<TSecond, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>;
}

/// <summary>
///     An interface for adding a <c>JOIN</c> clause to the query.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IJoinBuilder<TFirst, TSecond, TThird, TReturn> : IWhereBuilder<TFirst, TSecond, TThird, TReturn>;
