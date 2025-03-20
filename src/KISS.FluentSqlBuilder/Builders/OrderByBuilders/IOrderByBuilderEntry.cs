namespace KISS.FluentSqlBuilder.Builders.OrderByBuilders;

/// <summary>
///     An interface for adding a <c>ORDER BY</c> clause to the query.
///     Provides the entry point for specifying sort criteria in SQL queries.
/// </summary>
public interface IOrderByBuilderEntry : ISqlBuilder;

/// <summary>
///     An interface for adding a <c>ORDER BY</c> clause to the query with a single recordset type.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IOrderByBuilderEntry<TRecordset, TReturn> :
    ISqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>ORDER BY</c> clause to the query.
    /// </summary>
    /// <param name="selector">The expression selecting the columns to sort by.</param>
    /// <typeparam name="TKey">The type of the sort key, must implement IComparable.</typeparam>
    /// <returns>The <see cref="IOrderByBuilder{TRecordset, TReturn}" /> instance for method chaining.</returns>
    IOrderByBuilder<TRecordset, TReturn> OrderBy<TKey>(Expression<Func<TRecordset, TKey>> selector)
        where TKey : IComparable<TKey>;
}

/// <summary>
///     An interface for adding a <c>ORDER BY</c> clause to the query with two recordset types.
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
    /// <param name="selector">The expression selecting the columns to sort by.</param>
    /// <typeparam name="TKey">The type of the sort key, must implement IComparable.</typeparam>
    /// <returns>The <see cref="IOrderByBuilder{TFirst, TSecond, TReturn}" /> instance for method chaining.</returns>
    IOrderByBuilder<TFirst, TSecond, TReturn> OrderBy<TKey>(Expression<Func<TFirst, TSecond, TKey>> selector)
        where TKey : IComparable<TKey>;
}

/// <summary>
///     An interface for adding a <c>ORDER BY</c> clause to the query with three recordset types.
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
    /// <param name="selector">The expression selecting the columns to sort by.</param>
    /// <typeparam name="TKey">The type of the sort key, must implement IComparable.</typeparam>
    /// <returns>The <see cref="IOrderByBuilder{TFirst, TSecond, TThird, TReturn}" /> instance for method chaining.</returns>
    IOrderByBuilder<TFirst, TSecond, TThird, TReturn>
        OrderBy<TKey>(Expression<Func<TFirst, TSecond, TThird, TKey>> selector)
        where TKey : IComparable<TKey>;
}

/// <summary>
///     An interface for adding a <c>ORDER BY</c> clause to grouped SQL queries.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IGroupOrderByBuilderEntry<TRecordset, TReturn> :
    IGroupSqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>ORDER BY</c> clause to the grouped query.
    /// </summary>
    /// <param name="selector">The expression selecting the columns to sort by.</param>
    /// <typeparam name="TKey">The type of the sort key, must implement IComparable.</typeparam>
    /// <returns>The <see cref="IGroupOrderByBuilder{TRecordset, TReturn}" /> instance for method chaining.</returns>
    IGroupOrderByBuilder<TRecordset, TReturn> OrderBy<TKey>(Expression<Func<TRecordset, TKey>> selector)
        where TKey : IComparable<TKey>;
}
