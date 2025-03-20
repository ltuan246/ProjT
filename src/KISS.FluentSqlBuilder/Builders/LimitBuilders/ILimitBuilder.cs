namespace KISS.FluentSqlBuilder.Builders.LimitBuilders;

/// <summary>
///     An interface for building <c>LIMIT</c> clauses in SQL queries.
///     Provides functionality to specify the maximum number of rows to return.
/// </summary>
public interface ILimitBuilder : ISqlBuilder;

/// <summary>
///     An interface for building <c>LIMIT</c> clauses in SQL queries with a single recordset type.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ILimitBuilder<TRecordset, TReturn> : ISqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>LIMIT</c> clause to the query.
    /// </summary>
    /// <param name="rows">The maximum number of rows to return.</param>
    /// <returns>The <see cref="IOffsetBuilder{TRecordset, TReturn}" /> instance for method chaining.</returns>
    IOffsetBuilder<TRecordset, TReturn> Limit(int rows);
}

/// <summary>
///     An interface for building <c>LIMIT</c> clauses in SQL queries with two recordset types.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ILimitBuilder<TFirst, TSecond, TReturn> : ISqlBuilder<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends the <c>LIMIT</c> clause to the query.
    /// </summary>
    /// <param name="rows">The maximum number of rows to return.</param>
    /// <returns>The <see cref="IOffsetBuilder{TFirst, TSecond, TReturn}" /> instance for method chaining.</returns>
    IOffsetBuilder<TFirst, TSecond, TReturn> Limit(int rows);
}

/// <summary>
///     An interface for building <c>LIMIT</c> clauses in SQL queries with three recordset types.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ILimitBuilder<TFirst, TSecond, TThird, TReturn> : ISqlBuilder<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Appends the <c>LIMIT</c> clause to the query.
    /// </summary>
    /// <param name="rows">The maximum number of rows to return.</param>
    /// <returns>The <see cref="IOffsetBuilder{TFirst, TSecond, TThird, TReturn}" /> instance for method chaining.</returns>
    IOffsetBuilder<TFirst, TSecond, TThird, TReturn> Limit(int rows);
}

/// <summary>
///     An interface for building <c>LIMIT</c> clauses in grouped SQL queries.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IGroupLimitBuilder<TRecordset, TReturn> : IGroupSqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>LIMIT</c> clause to the grouped query.
    /// </summary>
    /// <param name="rows">The maximum number of rows to return.</param>
    /// <returns>The <see cref="IGroupOffsetBuilder{TRecordset, TReturn}" /> instance for method chaining.</returns>
    IGroupOffsetBuilder<TRecordset, TReturn> Limit(int rows);
}
