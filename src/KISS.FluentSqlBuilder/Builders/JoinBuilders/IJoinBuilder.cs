namespace KISS.FluentSqlBuilder.Builders.JoinBuilders;

/// <summary>
///     An interface for adding <c>JOIN</c> clauses to a query with a single table.
///     This interface enables type-safe construction of joins between tables.
/// </summary>
/// <typeparam name="TRecordset">The type representing the base table being queried.</typeparam>
/// <typeparam name="TReturn">The type of the result set after joining and mapping.</typeparam>
public interface IJoinBuilder<TRecordset, TReturn> : IWhereBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends an <c>INNER JOIN</c> clause to the query for a one-to-one relationship.
    ///     This method joins the current table with another table based on matching keys.
    /// </summary>
    /// <typeparam name="TRelation">The type of the table to join with.</typeparam>
    /// <param name="leftKeySelector">
    ///     An expression selecting the join key from the current table.
    ///     For example: <c>order => order.CustomerId</c>.
    /// </param>
    /// <param name="rightKeySelector">
    ///     An expression selecting the join key from the related table.
    ///     For example: <c>customer => customer.Id</c>.
    /// </param>
    /// <param name="mapSelector">
    ///     An expression defining how the joined table is mapped in the result.
    ///     For example: <c>result => result.Customer</c>.
    /// </param>
    /// <returns>
    ///     An <see cref="IJoinBuilder{TFirst, TSecond, TReturn}" /> instance that can be used
    ///     to add more joins or WHERE clauses.
    /// </returns>
    IJoinBuilder<TRecordset, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TRecordset, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, TRelation?>> mapSelector);

    /// <summary>
    ///     Appends an <c>INNER JOIN</c> clause to the query for a one-to-many relationship.
    ///     This method joins the current table with another table, expecting multiple matches.
    /// </summary>
    /// <typeparam name="TRelation">The type of the table to join with.</typeparam>
    /// <param name="leftKeySelector">
    ///     An expression selecting the join key from the current table.
    ///     For example: <c>customer => customer.Id</c>.
    /// </param>
    /// <param name="rightKeySelector">
    ///     An expression selecting the join key from the related table.
    ///     For example: <c>order => order.CustomerId</c>.
    /// </param>
    /// <param name="mapSelector">
    ///     An expression defining how the joined table is mapped in the result.
    ///     For example: <c>result => result.Orders</c>.
    /// </param>
    /// <returns>
    ///     An <see cref="IJoinBuilder{TFirst, TSecond, TReturn}" /> instance that can be used
    ///     to add more joins or WHERE clauses.
    /// </returns>
    IJoinBuilder<TRecordset, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TRecordset, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, List<TRelation>?>> mapSelector);
}

/// <summary>
///     An interface for adding <c>JOIN</c> clauses to a query with two tables.
///     This interface enables type-safe construction of joins between multiple tables.
/// </summary>
/// <typeparam name="TFirst">The type representing the first table in the join chain.</typeparam>
/// <typeparam name="TSecond">The type representing the second table in the join chain.</typeparam>
/// <typeparam name="TReturn">The type of the result set after joining and mapping.</typeparam>
public interface IJoinBuilder<TFirst, TSecond, TReturn> : IWhereBuilder<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends an <c>INNER JOIN</c> clause to the query for a one-to-one relationship,
    ///     joining from the first table.
    /// </summary>
    /// <typeparam name="TRelation">The type of the table to join with.</typeparam>
    /// <param name="leftKeySelector">
    ///     An expression selecting the join key from the first table.
    ///     For example: <c>order => order.ProductId</c>.
    /// </param>
    /// <param name="rightKeySelector">
    ///     An expression selecting the join key from the related table.
    ///     For example: <c>product => product.Id</c>.
    /// </param>
    /// <param name="mapSelector">
    ///     An expression defining how the joined table is mapped in the result.
    ///     For example: <c>result => result.Product</c>.
    /// </param>
    /// <returns>
    ///     An <see cref="IJoinBuilder{TFirst, TSecond, TThird, TReturn}" /> instance that can be used
    ///     to add more joins or WHERE clauses.
    /// </returns>
    IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TFirst, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, TRelation?>> mapSelector);

    /// <summary>
    ///     Appends an <c>INNER JOIN</c> clause to the query for a one-to-many relationship,
    ///     joining from the first table.
    /// </summary>
    /// <typeparam name="TRelation">The type of the table to join with.</typeparam>
    /// <param name="leftKeySelector">
    ///     An expression selecting the join key from the first table.
    ///     For example: <c>product => product.Id</c>.
    /// </param>
    /// <param name="rightKeySelector">
    ///     An expression selecting the join key from the related table.
    ///     For example: <c>orderItem => orderItem.ProductId</c>.
    /// </param>
    /// <param name="mapSelector">
    ///     An expression defining how the joined table is mapped in the result.
    ///     For example: <c>result => result.OrderItems</c>.
    /// </param>
    /// <returns>
    ///     An <see cref="IJoinBuilder{TFirst, TSecond, TThird, TReturn}" /> instance that can be used
    ///     to add more joins or WHERE clauses.
    /// </returns>
    IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TFirst, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, List<TRelation>?>> mapSelector);

    /// <summary>
    ///     Appends an <c>INNER JOIN</c> clause to the query for a one-to-one relationship,
    ///     joining from the second table.
    /// </summary>
    /// <typeparam name="TRelation">The type of the table to join with.</typeparam>
    /// <param name="leftKeySelector">
    ///     An expression selecting the join key from the second table.
    ///     For example: <c>customer => customer.CountryId</c>.
    /// </param>
    /// <param name="rightKeySelector">
    ///     An expression selecting the join key from the related table.
    ///     For example: <c>country => country.Id</c>.
    /// </param>
    /// <param name="mapSelector">
    ///     An expression defining how the joined table is mapped in the result.
    ///     For example: <c>result => result.Country</c>.
    /// </param>
    /// <returns>
    ///     An <see cref="IJoinBuilder{TFirst, TSecond, TThird, TReturn}" /> instance that can be used
    ///     to add more joins or WHERE clauses.
    /// </returns>
    IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TSecond, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, TRelation?>> mapSelector);

    /// <summary>
    ///     Appends an <c>INNER JOIN</c> clause to the query for a one-to-many relationship,
    ///     joining from the second table.
    /// </summary>
    /// <typeparam name="TRelation">The type of the table to join with.</typeparam>
    /// <param name="leftKeySelector">
    ///     An expression selecting the join key from the second table.
    ///     For example: <c>customer => customer.Id</c>.
    /// </param>
    /// <param name="rightKeySelector">
    ///     An expression selecting the join key from the related table.
    ///     For example: <c>address => address.CustomerId</c>.
    /// </param>
    /// <param name="mapSelector">
    ///     An expression defining how the joined table is mapped in the result.
    ///     For example: <c>result => result.Addresses</c>.
    /// </param>
    /// <returns>
    ///     An <see cref="IJoinBuilder{TFirst, TSecond, TThird, TReturn}" /> instance that can be used
    ///     to add more joins or WHERE clauses.
    /// </returns>
    IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TSecond, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, List<TRelation>?>> mapSelector);
}

/// <summary>
///     An interface for adding <c>JOIN</c> clauses to a query with three tables.
///     This interface represents the final stage of join operations, as the query
///     builder supports a maximum of three joined tables.
/// </summary>
/// <typeparam name="TFirst">The type representing the first table in the join chain.</typeparam>
/// <typeparam name="TSecond">The type representing the second table in the join chain.</typeparam>
/// <typeparam name="TThird">The type representing the third table in the join chain.</typeparam>
/// <typeparam name="TReturn">The type of the result set after joining and mapping.</typeparam>
public interface IJoinBuilder<TFirst, TSecond, TThird, TReturn> : IWhereBuilder<TFirst, TSecond, TThird, TReturn>;
