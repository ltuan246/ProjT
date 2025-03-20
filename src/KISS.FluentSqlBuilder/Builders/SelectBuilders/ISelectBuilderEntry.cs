namespace KISS.FluentSqlBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface for building <c>SELECT</c> clauses in SQL queries.
///     Provides functionality to specify columns and expressions to retrieve.
/// </summary>
public interface ISelectBuilderEntry;

/// <summary>
///     An interface for building <c>SELECT</c> clauses in SQL queries with a single recordset type.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISelectBuilderEntry<TRecordset, TReturn> :
    ISqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>SELECT</c> clause to the query.
    /// </summary>
    /// <param name="selector">The expression specifying columns to select.</param>
    /// <returns>The <see cref="ISelectBuilder{TRecordset, TReturn}" /> instance for method chaining.</returns>
    ISelectBuilder<TRecordset, TReturn> Select(Expression<Func<TRecordset, TReturn>> selector);
}

/// <summary>
///     An interface for building <c>SELECT</c> clauses in SQL queries with two recordset types.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISelectBuilderEntry<TFirst, TSecond, TReturn> :
    ISqlBuilder<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends the <c>SELECT</c> clause to the query.
    /// </summary>
    /// <param name="selector">The expression specifying columns to select.</param>
    /// <returns>The <see cref="ISelectBuilderEntry{TFirst, TSecond, TReturn}" /> instance for method chaining.</returns>
    ISelectBuilder<TFirst, TSecond, TReturn> Select(Expression<Func<TFirst, TSecond, TReturn>> selector);
}

/// <summary>
///     An interface for building <c>SELECT</c> clauses in SQL queries with three recordset types.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISelectBuilderEntry<TFirst, TSecond, TThird, TReturn> :
    ISqlBuilder<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Appends the <c>SELECT</c> clause to the query.
    /// </summary>
    /// <param name="selector">The expression specifying columns to select.</param>
    /// <returns>The <see cref="ISelectBuilder{TFirst, TSecond, TThird, TReturn}" /> instance for method chaining.</returns>
    ISelectBuilder<TFirst, TSecond, TThird, TReturn>
        Select(Expression<Func<TFirst, TSecond, TThird, TReturn>> selector);
}
