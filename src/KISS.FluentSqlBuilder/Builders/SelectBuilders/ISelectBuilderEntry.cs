namespace KISS.FluentSqlBuilder.Builders.SelectBuilders;

/// <summary>
///     An interface for adding a <c>SELECT</c> clause to the query.
/// </summary>
public interface ISelectBuilderEntry;

/// <summary>
///     An interface for adding a <c>SELECT</c> clause to the query.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISelectBuilderEntry<TRecordset, TReturn> :
    ISqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>SELECT</c> clause to the builder.
    /// </summary>
    /// <param name="selector">The table columns.</param>
    /// <returns>The <see cref="ISelectBuilder{TRecordset, TReturn}" /> instance.</returns>
    ISelectBuilder<TRecordset, TReturn> Select(Expression<Func<TRecordset, TReturn>> selector);
}

/// <summary>
///     An interface for adding a <c>SELECT</c> clause to the query.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISelectBuilderEntry<TFirst, TSecond, TReturn> :
    ISqlBuilder<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends the <c>SELECT</c> clause to the builder.
    /// </summary>
    /// <param name="selector">The table columns.</param>
    /// <returns>The <see cref="ISelectBuilder{TRecordset, TReturn}" /> instance.</returns>
    ISelectBuilder<TFirst, TSecond, TReturn> Select(Expression<Func<TFirst, TSecond, TReturn>> selector);
}

/// <summary>
///     An interface for adding a <c>SELECT</c> clause to the query.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ISelectBuilderEntry<TFirst, TSecond, TThird, TReturn> :
    ISqlBuilder<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Appends the <c>SELECT</c> clause to the builder.
    /// </summary>
    /// <param name="selector">The table columns.</param>
    /// <returns>The <see cref="ISelectBuilder{TRecordset, TReturn}" /> instance.</returns>
    ISelectBuilder<TFirst, TSecond, TThird, TReturn>
        Select(Expression<Func<TFirst, TSecond, TThird, TReturn>> selector);
}
