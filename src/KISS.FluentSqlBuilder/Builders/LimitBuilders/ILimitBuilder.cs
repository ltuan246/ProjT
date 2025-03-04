namespace KISS.FluentSqlBuilder.Builders.LimitBuilders;

/// <summary>
///     An interface for building <c>LIMIT</c> clauses.
/// </summary>
public interface ILimitBuilder : ISqlBuilder;

/// <summary>
///     An interface for building <c>LIMIT</c> clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ILimitBuilder<TRecordset, TReturn>
    : ISqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>LIMIT</c> clause and the <paramref name="rows" /> to the builder.
    /// </summary>
    /// <param name="rows">The number of rows to fetch.</param>
    /// <returns>The <see cref="IOffsetBuilder" /> instance.</returns>
    IOffsetBuilder<TRecordset, TReturn> Limit(int rows);
}

/// <summary>
///     An interface for building <c>LIMIT</c> clauses.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ILimitBuilder<TFirst, TSecond, TReturn>
    : ISqlBuilder<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends the <c>LIMIT</c> clause and the <paramref name="rows" /> to the builder.
    /// </summary>
    /// <param name="rows">The number of rows to fetch.</param>
    /// <returns>The <see cref="IOffsetBuilder" /> instance.</returns>
    IOffsetBuilder<TFirst, TSecond, TReturn> Limit(int rows);
}

/// <summary>
///     An interface for building <c>LIMIT</c> clauses.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface ILimitBuilder<TFirst, TSecond, TThird, TReturn>
    : ISqlBuilder<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Appends the <c>LIMIT</c> clause and the <paramref name="rows" /> to the builder.
    /// </summary>
    /// <param name="rows">The number of rows to fetch.</param>
    /// <returns>The <see cref="IOffsetBuilder" /> instance.</returns>
    IOffsetBuilder<TFirst, TSecond, TThird, TReturn> Limit(int rows);
}

/// <summary>
///     An interface for building <c>LIMIT</c> clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IGroupLimitBuilder<TRecordset, TReturn>
    : IGroupSqlBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>LIMIT</c> clause and the <paramref name="rows" /> to the builder.
    /// </summary>
    /// <param name="rows">The number of rows to fetch.</param>
    /// <returns>The <see cref="IOffsetBuilder" /> instance.</returns>
    IGroupOffsetBuilder<TRecordset, TReturn> Limit(int rows);
}
