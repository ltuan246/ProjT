namespace KISS.FluentSqlBuilder.Builders;

/// <summary>
///     Defines the base interface for building SQL queries with a fluent interface.
///     This interface serves as the foundation for constructing SQL queries by
///     combining various SQL clauses in a type-safe manner.
/// </summary>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface IQueryBuilder<TReturn> : ISelectFromBuilder<TReturn>;

/// <summary>
///     Extends the query builder interface to support single-table queries with
///     additional SQL clauses like JOIN, ORDER BY, and OFFSET.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface IQueryBuilder<TRecordset, TReturn> :
    IJoinBuilder<TRecordset, TReturn>,
    ISelectBuilder<TRecordset, TReturn>,
    IOrderByBuilder<TRecordset, TReturn>,
    IOffsetBuilder<TRecordset, TReturn>;

/// <summary>
///     Extends the query builder interface to support two-table queries with
///     additional SQL clauses like JOIN, ORDER BY, and OFFSET.
/// </summary>
/// <typeparam name="TFirst">The type of the first table in the query.</typeparam>
/// <typeparam name="TSecond">The type of the second table in the query.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface IQueryBuilder<TFirst, TSecond, TReturn> :
    IJoinBuilder<TFirst, TSecond, TReturn>,
    ISelectBuilder<TFirst, TSecond, TReturn>,
    IOrderByBuilder<TFirst, TSecond, TReturn>,
    IOffsetBuilder<TFirst, TSecond, TReturn>;

/// <summary>
///     Extends the query builder interface to support three-table queries with
///     additional SQL clauses like JOIN, ORDER BY, and OFFSET.
/// </summary>
/// <typeparam name="TFirst">The type of the first table in the query.</typeparam>
/// <typeparam name="TSecond">The type of the second table in the query.</typeparam>
/// <typeparam name="TThird">The type of the third table in the query.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface IQueryBuilder<TFirst, TSecond, TThird, TReturn> :
    IJoinBuilder<TFirst, TSecond, TThird, TReturn>,
    ISelectBuilder<TFirst, TSecond, TThird, TReturn>,
    IOrderByBuilder<TFirst, TSecond, TThird, TReturn>,
    IOffsetBuilder<TFirst, TSecond, TThird, TReturn>;

/// <summary>
///     Defines a specialized query builder interface for queries that include
///     GROUP BY clauses and related aggregations.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface IGroupQueryBuilder<TRecordset, TReturn> :
    IGroupByBuilder<TRecordset, TReturn>,
    IGroupOrderByBuilder<TRecordset, TReturn>,
    IGroupOffsetBuilder<TRecordset, TReturn>;
