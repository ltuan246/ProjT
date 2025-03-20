namespace KISS.FluentSqlBuilder.Builders.SelectBuilders;

/// <summary>
///     Defines the base interface for building SQL SELECT queries. This interface serves
///     as the foundation for constructing SELECT statements in a fluent manner.
/// </summary>
public interface ISelectBuilder;

/// <summary>
///     Defines an interface for building SELECT queries for a single table or view.
///     This interface extends the base ISelectBuilder to provide type-safe query building
///     for a single recordset.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface ISelectBuilder<TRecordset, TReturn> :
    IOrderByBuilderEntry<TRecordset, TReturn>;

/// <summary>
///     Defines an interface for building SELECT queries that join two tables.
///     This interface extends the base ISelectBuilder to provide type-safe query building
///     for queries involving two tables.
/// </summary>
/// <typeparam name="TFirst">The type of the first table in the query.</typeparam>
/// <typeparam name="TSecond">The type of the second table in the query.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface ISelectBuilder<TFirst, TSecond, TReturn> :
    IOrderByBuilderEntry<TFirst, TSecond, TReturn>;

/// <summary>
///     Defines an interface for building SELECT queries that join three tables.
///     This interface extends the base ISelectBuilder to provide type-safe query building
///     for complex queries involving three tables.
/// </summary>
/// <typeparam name="TFirst">The type of the first table in the query.</typeparam>
/// <typeparam name="TSecond">The type of the second table in the query.</typeparam>
/// <typeparam name="TThird">The type of the third table in the query.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface ISelectBuilder<TFirst, TSecond, TThird, TReturn> :
    IOrderByBuilderEntry<TFirst, TSecond, TThird, TReturn>;

/// <summary>
///     Defines an interface for building SELECT queries that include aggregation operations
///     and grouping. This interface is specifically designed for queries that use GROUP BY
///     and aggregate functions.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public interface IGroupSelectBuilder<TRecordset, TReturn> :
    IGroupOrderByBuilderEntry<TRecordset, TReturn>
{
    /// <summary>
    ///     Adds an aggregate function to the SELECT clause of a grouped query.
    ///     This method allows you to specify aggregate operations like COUNT, SUM, AVG, etc.
    /// </summary>
    /// <param name="selector">An expression defining the aggregation operation to perform.</param>
    /// <param name="alias">The alias to use for the aggregate result in the query output.</param>
    /// <returns>The <see cref="IGroupSelectBuilder{TRecordset, TReturn}" /> instance for method chaining.</returns>
    IGroupSelectBuilder<TRecordset, TReturn> SelectAggregate(
        Expression<Func<AggregationBuilder<TRecordset>, AggregationComparer<TRecordset>>> selector,
        string alias);
}
