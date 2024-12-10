namespace KISS.FluentSqlBuilder.Builders;

/// <summary>
///     An interface that defines the query-building method shared across clauses,
///     each corresponding to different SQL clauses.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IQueryBuilder<TRecordset, TReturn> :
    IJoinBuilder<TRecordset, TReturn>,
    ISelectBuilder<TRecordset, TReturn>,
    IOrderByBuilder<TRecordset, TReturn>,
    IOffsetBuilder<TRecordset, TReturn>;

/// <summary>
///     An interface that defines the query-building method shared across clauses,
///     each corresponding to different SQL clauses.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IQueryBuilder<TFirst, TSecond, TReturn> :
    IJoinBuilder<TFirst, TSecond, TReturn>,
    ISelectBuilder<TFirst, TSecond, TReturn>,
    IOrderByBuilder<TFirst, TSecond, TReturn>,
    IOffsetBuilder<TFirst, TSecond, TReturn>;

/// <summary>
///     An interface that defines the query-building method shared across clauses,
///     each corresponding to different SQL clauses.
/// </summary>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IQueryBuilder<TFirst, TSecond, TThird, TReturn> :
    IJoinBuilder<TFirst, TSecond, TThird, TReturn>,
    ISelectBuilder<TFirst, TSecond, TThird, TReturn>,
    IOrderByBuilder<TFirst, TSecond, TThird, TReturn>,
    IOffsetBuilder<TFirst, TSecond, TThird, TReturn>;

/// <summary>
///     An interface that defines the query-building method shared across clauses,
///     each corresponding to different SQL clauses.
/// </summary>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public interface IQueryBuilderEntry<TReturn> :
    ISelectFromBuilderEntry<TReturn>;
