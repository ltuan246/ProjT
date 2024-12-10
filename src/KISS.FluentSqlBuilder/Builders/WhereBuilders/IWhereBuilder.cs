namespace KISS.FluentSqlBuilder.Builders.WhereBuilders;

/// <summary>
///     An interface for building <c>WHERE</c> clauses.
/// </summary>
public interface IWhereBuilder;

/// <summary>
///     An interface for building <c>WHERE</c> clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IWhereBuilder<TRecordset, TReturn> :
    ISelectBuilderEntry<TRecordset, TReturn>,
    IOrderByBuilderEntry<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="predicate">Filters a sequence of values based on a predicate.</param>
    /// <returns>The <see cref="IWhereBuilder{TRecordset, TReturn}" /> instance.</returns>
    IWhereBuilder<TRecordset, TReturn> Where(Expression<Func<TRecordset, bool>> predicate);
}

/// <summary>
///     An interface for building <c>WHERE</c> clauses.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IWhereBuilder<TFirst, TSecond, TReturn> :
    ISelectBuilderEntry<TFirst, TSecond, TReturn>,
    IOrderByBuilderEntry<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="predicate">Filters a sequence of values based on a predicate.</param>
    /// <returns>The <see cref="IWhereBuilder{TFirst, TSecond, TReturn}" /> instance.</returns>
    IWhereBuilder<TFirst, TSecond, TReturn> Where(Expression<Func<TFirst, bool>> predicate);

    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="predicate">Filters a sequence of values based on a predicate.</param>
    /// <returns>The <see cref="IWhereBuilder{TFirst, TSecond, TReturn}" /> instance.</returns>
    IWhereBuilder<TFirst, TSecond, TReturn> Where(Expression<Func<TSecond, bool>> predicate);
}

/// <summary>
///     An interface for building <c>WHERE</c> clauses.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IWhereBuilder<TFirst, TSecond, TThird, TReturn> :
    ISelectBuilderEntry<TFirst, TSecond, TThird, TReturn>,
    IOrderByBuilderEntry<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="predicate">Filters a sequence of values based on a predicate.</param>
    /// <returns>The <see cref="IWhereBuilder{TFirst, TSecond, TThird, TReturn}" /> instance.</returns>
    IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TFirst, bool>> predicate);

    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="predicate">Filters a sequence of values based on a predicate.</param>
    /// <returns>The <see cref="IWhereBuilder{TFirst, TSecond, TThird, TReturn}" /> instance.</returns>
    IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TSecond, bool>> predicate);

    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="predicate">Filters a sequence of values based on a predicate.</param>
    /// <returns>The <see cref="IWhereBuilder{TFirst, TSecond, TThird, TReturn}" /> instance.</returns>
    IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TThird, bool>> predicate);
}
