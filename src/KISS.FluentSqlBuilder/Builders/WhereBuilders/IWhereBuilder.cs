namespace KISS.FluentSqlBuilder.Builders.WhereBuilders;

/// <summary>
///     An interface for building <c>WHERE</c> clauses in SQL queries.
///     Provides functionality to specify conditions for filtering results.
/// </summary>
public interface IWhereBuilder;

/// <summary>
///     An interface for building <c>WHERE</c> clauses in SQL queries with a single recordset type.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IWhereBuilder<TRecordset, TReturn> :
    IGroupByBuilderEntry<TRecordset, TReturn>,
    ISelectBuilderEntry<TRecordset, TReturn>,
    IOrderByBuilderEntry<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="predicate">The expression specifying the condition.</param>
    /// <returns>The <see cref="IWhereBuilder{TRecordset, TReturn}" /> instance for method chaining.</returns>
    IWhereBuilder<TRecordset, TReturn> Where(Expression<Func<TRecordset, bool>> predicate);

    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <param name="predicate">The expression specifying the condition.</param>
    /// <returns>The <see cref="IWhereBuilder{TRecordset, TReturn}" /> instance for method chaining.</returns>
    IWhereBuilder<TRecordset, TReturn> Where(bool condition, Expression<Func<TRecordset, bool>> predicate);
}

/// <summary>
///     An interface for building <c>WHERE</c> clauses in SQL queries with two recordset types.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IWhereBuilder<TFirst, TSecond, TReturn> :
    IGroupByBuilderEntry<TFirst, TSecond, TReturn>,
    ISelectBuilderEntry<TFirst, TSecond, TReturn>,
    IOrderByBuilderEntry<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="predicate">The expression specifying the condition.</param>
    /// <returns>The <see cref="IWhereBuilder{TFirst, TSecond, TReturn}" /> instance for method chaining.</returns>
    IWhereBuilder<TFirst, TSecond, TReturn> Where(Expression<Func<TFirst, bool>> predicate);

    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="predicate">The expression specifying the condition.</param>
    /// <returns>The <see cref="IWhereBuilder{TFirst, TSecond, TReturn}" /> instance for method chaining.</returns>
    IWhereBuilder<TFirst, TSecond, TReturn> Where(Expression<Func<TSecond, bool>> predicate);
}

/// <summary>
///     An interface for building <c>WHERE</c> clauses in SQL queries with three recordset types.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IWhereBuilder<TFirst, TSecond, TThird, TReturn> :
    IGroupByBuilderEntry<TFirst, TSecond, TThird, TReturn>,
    ISelectBuilderEntry<TFirst, TSecond, TThird, TReturn>,
    IOrderByBuilderEntry<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="predicate">The expression specifying the condition.</param>
    /// <returns>The <see cref="IWhereBuilder{TFirst, TSecond, TThird, TReturn}" /> instance for method chaining.</returns>
    IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TFirst, bool>> predicate);

    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="condition">The value to determine whether the method should be executed.</param>
    /// <param name="predicate">The expression specifying the condition.</param>
    /// <returns>The <see cref="IWhereBuilder{TFirst, TSecond, TThird, TReturn}" /> instance for method chaining.</returns>
    IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(bool condition, Expression<Func<TFirst, bool>> predicate);

    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="predicate">The expression specifying the condition.</param>
    /// <returns>The <see cref="IWhereBuilder{TFirst, TSecond, TThird, TReturn}" /> instance.</returns>
    IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TSecond, bool>> predicate);

    /// <summary>
    ///     Appends the <c>WHERE</c> clause (or the AND clause, if a WHERE clause is present) to the query.
    /// </summary>
    /// <param name="predicate">The expression specifying the condition.</param>
    /// <returns>The <see cref="IWhereBuilder{TFirst, TSecond, TThird, TReturn}" /> instance for method chaining.</returns>
    IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TThird, bool>> predicate);
}
