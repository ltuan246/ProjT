namespace KISS.FluentSqlBuilder.Builders.OffsetBuilders;

/// <summary>
///     An interface for building <c>OFFSET</c> clauses.
/// </summary>
public interface IOffsetBuilder : ISqlBuilder;

/// <summary>
///     An interface for building <c>OFFSET</c> clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IOffsetBuilder<TRecordset, TReturn>
{
    /// <summary>
    ///     Appends the <c>OFFSET</c> clause and the <paramref name="offset" /> to the builder.
    /// </summary>
    /// <param name="offset">The number of rows to skip.</param>
    /// <returns>The <see cref="ISqlBuilder" /> instance.</returns>
    ISqlBuilder<TRecordset, TReturn> Offset(int offset);
}

/// <summary>
///     An interface for building <c>OFFSET</c> clauses.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IOffsetBuilder<TFirst, TSecond, TReturn>
{
    /// <summary>
    ///     Appends the <c>OFFSET</c> clause and the <paramref name="offset" /> to the builder.
    /// </summary>
    /// <param name="offset">The number of rows to skip.</param>
    /// <returns>The <see cref="ISqlBuilder" /> instance.</returns>
    ISqlBuilder<TFirst, TSecond, TReturn> Offset(int offset);
}

/// <summary>
///     An interface for building <c>OFFSET</c> clauses.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IOffsetBuilder<TFirst, TSecond, TThird, TReturn>
{
    /// <summary>
    ///     Appends the <c>OFFSET</c> clause and the <paramref name="offset" /> to the builder.
    /// </summary>
    /// <param name="offset">The number of rows to skip.</param>
    /// <returns>The <see cref="ISqlBuilder" /> instance.</returns>
    ISqlBuilder<TFirst, TSecond, TThird, TReturn> Offset(int offset);
}
