namespace KISS.FluentSqlBuilder.Builders.OrderByBuilders;

/// <summary>
///     An interface for adding a <c>ORDER BY</c> clause to the query.
/// </summary>
public interface IOrderByBuilderEntry : ISqlBuilder;

/// <summary>
///     An interface for adding a <c>ORDER BY</c> clause to the query.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IOrderByBuilderEntry<TRecordset, TReturn> :
    ISqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>ORDER BY</c> clause to the query.
    /// </summary>
    /// <param name="selector">The table columns.</param>
    /// <typeparam name="TKey">The order keys.</typeparam>
    /// <returns>The <see cref="IOrderByBuilder{TRecordset, TReturn}" /> instance.</returns>
    IOrderByBuilder<TRecordset, TReturn> OrderBy<TKey>(Expression<Func<TRecordset, TKey>> selector)
        where TKey : IComparable<TKey>;
}

/// <summary>
///     An interface for adding a <c>ORDER BY</c> clause to the query.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IOrderByBuilderEntry<TFirst, TSecond, TReturn> :
    ISqlBuilder<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends the <c>ORDER BY</c> clause to the query.
    /// </summary>
    /// <param name="selector">The table columns.</param>
    /// <typeparam name="TKey">The order keys.</typeparam>
    /// <returns>The <see cref="IOrderByBuilder{TRecordset, TReturn}" /> instance.</returns>
    IOrderByBuilder<TFirst, TSecond, TReturn> OrderBy<TKey>(Expression<Func<TFirst, TSecond, TKey>> selector)
        where TKey : IComparable<TKey>;
}

/// <summary>
///     An interface for adding a <c>ORDER BY</c> clause to the query.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IOrderByBuilderEntry<TFirst, TSecond, TThird, TReturn> :
    ISqlBuilder<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Appends the <c>ORDER BY</c> clause to the query.
    /// </summary>
    /// <param name="selector">The table columns.</param>
    /// <typeparam name="TKey">The order keys.</typeparam>
    /// <returns>The <see cref="IOrderByBuilder{TRecordset, TReturn}" /> instance.</returns>
    IOrderByBuilder<TFirst, TSecond, TThird, TReturn>
        OrderBy<TKey>(Expression<Func<TFirst, TSecond, TThird, TKey>> selector)
        where TKey : IComparable<TKey>;
}
