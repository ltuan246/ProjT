namespace KISS.FluentSqlBuilder.Builders.OrderByBuilders;

/// <summary>
///     An interface for building <c>ORDER BY</c> clauses in SQL queries.
///     Provides functionality to sort query results and supports pagination.
/// </summary>
public interface IOrderByBuilder : IFetchBuilder, IOffsetRowsBuilder, ILimitBuilder;

/// <summary>
///     An interface for building <c>ORDER BY</c> clauses in SQL queries with a single recordset type.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IOrderByBuilder<TRecordset, TReturn>
    : IFetchBuilder, IOffsetRowsBuilder, ILimitBuilder<TRecordset, TReturn>;

/// <summary>
///     An interface for building <c>ORDER BY</c> clauses in SQL queries with two recordset types.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IOrderByBuilder<TFirst, TSecond, TReturn>
    : IFetchBuilder, IOffsetRowsBuilder, ILimitBuilder<TFirst, TSecond, TReturn>;

/// <summary>
///     An interface for building <c>ORDER BY</c> clauses in SQL queries with three recordset types.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IOrderByBuilder<TFirst, TSecond, TThird, TReturn>
    : IFetchBuilder, IOffsetRowsBuilder, ILimitBuilder<TFirst, TSecond, TThird, TReturn>;

/// <summary>
///     An interface for building <c>ORDER BY</c> clauses in grouped SQL queries.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IGroupOrderByBuilder<TRecordset, TReturn>
    : IFetchBuilder, IOffsetRowsBuilder, IGroupLimitBuilder<TRecordset, TReturn>;
