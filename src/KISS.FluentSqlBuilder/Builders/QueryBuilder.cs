namespace KISS.FluentSqlBuilder.Builders;

/// <summary>
///     Provides the base implementation for building SQL queries with a fluent interface.
///     This class serves as the entry point for constructing SQL queries and manages
///     the connection to the database.
/// </summary>
/// <param name="Connection">The database connection used to execute queries.</param>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public sealed record QueryBuilder<TReturn>(DbConnection Connection) : IQueryBuilder<TReturn>
{
    /// <inheritdoc />
    public IJoinBuilder<TRecordset, TReturn> From<TRecordset>()
    {
        var handler = new SelectHandler<TRecordset, TReturn>();
        return new QueryBuilder<TRecordset, TReturn>(Connection, handler);
    }
}

/// <summary>
///     Implements the query builder for single-table operations, providing methods
///     for constructing SQL queries with various clauses (SELECT, WHERE, JOIN, etc.).
/// </summary>
/// <param name="Connection">The database connection used to execute queries.</param>
/// <param name="Handler">The query handler that manages query construction and execution.</param>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public sealed record QueryBuilder<TRecordset, TReturn>(DbConnection Connection, QueryHandler Handler) :
    IQueryBuilder<TRecordset, TReturn>
{
    /// <inheritdoc />
    public IJoinBuilder<TRecordset, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TRecordset, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, TRelation?>> mapSelector)
    {
        Handler.SetNext(new OneToOneJoinHandler<TRecordset, TRelation, TReturn>(leftKeySelector.Body, rightKeySelector.Body, mapSelector.Body));
        return new QueryBuilder<TRecordset, TRelation, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IJoinBuilder<TRecordset, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TRecordset, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, List<TRelation>?>> mapSelector)
    {
        Handler.SetNext(new OneToManyJoinHandler<TRecordset, TRelation, TReturn>(leftKeySelector.Body, rightKeySelector.Body, mapSelector.Body));
        return new QueryBuilder<TRecordset, TRelation, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IWhereBuilder<TRecordset, TReturn> Where(Expression<Func<TRecordset, bool>> predicate)
    {
        Handler.SetNext(new WhereHandler(predicate.Body));
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TRecordset, TReturn> Where(bool condition, Expression<Func<TRecordset, bool>> predicate)
    {
        if (condition)
        {
            Handler.SetNext(new WhereHandler(predicate.Body));
        }

        return this;
    }

    /// <inheritdoc />
    public IGroupByBuilder<TRecordset, TReturn> GroupBy(Expression<Func<TRecordset, IComparable>> selector)
    {
        Handler.SetNext(new GroupByHandler(selector.Body));
        return new GroupQueryBuilder<TRecordset, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public ISelectBuilder<TRecordset, TReturn> Select(Expression<Func<TRecordset, TReturn>> selector)
    {
        Handler.SetNext(new NewSelectHandler<TRecordset, TReturn>(selector.Body));
        return this;
    }

    /// <inheritdoc />
    public IOrderByBuilder<TRecordset, TReturn> OrderBy<TKey>(Expression<Func<TRecordset, TKey>> selector)
        where TKey : IComparable<TKey>
    {
        Handler.SetNext(new OrderByHandler(selector.Body));
        return this;
    }

    /// <inheritdoc />
    public IOffsetBuilder<TRecordset, TReturn> Limit(int rows)
    {
        Handler.SetNext(new LimitHandler(rows));
        return this;
    }

    /// <inheritdoc />
    public ISqlBuilder<TRecordset, TReturn> Offset(int offset)
    {
        Handler.SetNext(new OffsetHandler(offset));
        return this;
    }

    /// <inheritdoc />
    public List<TReturn> ToList()
        => new CompositeQueryProxy<TReturn>().Create(Connection, Handler).GetList<TReturn>();
}

/// <summary>
///     Implements the query builder for grouped queries, providing methods for
///     constructing SQL queries with GROUP BY clauses and aggregations.
/// </summary>
/// <param name="Connection">The database connection used to execute queries.</param>
/// <param name="Handler">The query handler that manages query construction and execution.</param>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public sealed record GroupQueryBuilder<TRecordset, TReturn>(DbConnection Connection, QueryHandler Handler) :
    IGroupQueryBuilder<TRecordset, TReturn>
{
    /// <inheritdoc />
    public IGroupByBuilder<TRecordset, TReturn> ThenBy(Expression<Func<TRecordset, IComparable>> selector)
    {
        Handler.SetNext(new GroupByHandler(selector.Body));
        return this;
    }

    /// <inheritdoc />
    public IHavingBuilder<TRecordset, TReturn> Having(
        Expression<Func<AggregationBuilder<TRecordset>, bool>> condition)
    {
        Handler.SetNext(new HavingHandler(condition.Body));
        return this;
    }

    /// <inheritdoc />
    public IGroupSelectBuilder<TRecordset, TReturn> SelectAggregate(
        Expression<Func<AggregationBuilder<TRecordset>, AggregationComparer<TRecordset>>> selector,
        string alias)
    {
        Handler.SetNext(new SelectAggregateHandler(selector.Body, alias));
        return this;
    }

    /// <inheritdoc />
    public IGroupOrderByBuilder<TRecordset, TReturn> OrderBy<TKey>(Expression<Func<TRecordset, TKey>> selector)
        where TKey : IComparable<TKey>
    {
        Handler.SetNext(new OrderByHandler(selector.Body));
        return this;
    }

    /// <inheritdoc />
    public IGroupOffsetBuilder<TRecordset, TReturn> Limit(int rows)
    {
        Handler.SetNext(new LimitHandler(rows));
        return this;
    }

    /// <inheritdoc />
    public IGroupSqlBuilder<TRecordset, TReturn> Offset(int offset)
    {
        Handler.SetNext(new OffsetHandler(offset));
        return this;
    }

    /// <inheritdoc />
    public Dictionary<ITuple, List<TReturn>> ToDictionary() =>
        new CompositeQueryProxy<TReturn>().Create(Connection, Handler).GetDictionary<TReturn>();
}

/// <summary>
///     Implements the query builder for two-table operations, providing methods
///     for constructing SQL queries that join two tables.
/// </summary>
/// <param name="Connection">The database connection used to execute queries.</param>
/// <param name="Handler">The query handler that manages query construction and execution.</param>
/// <typeparam name="TFirst">The type of the first table in the query.</typeparam>
/// <typeparam name="TSecond">The type of the second table in the query.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public sealed record QueryBuilder<TFirst, TSecond, TReturn>(DbConnection Connection, QueryHandler Handler) :
    IQueryBuilder<TFirst, TSecond, TReturn>
{
    /// <inheritdoc />
    public IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TFirst, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, TRelation?>> mapSelector)
    {
        Handler.SetNext(new OneToOneJoinHandler<TFirst, TRelation, TReturn>(leftKeySelector.Body, rightKeySelector.Body, mapSelector.Body));
        return new QueryBuilder<TFirst, TSecond, TRelation, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TFirst, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, List<TRelation>?>> mapSelector)
    {
        Handler.SetNext(new OneToManyJoinHandler<TFirst, TRelation, TReturn>(leftKeySelector.Body, rightKeySelector.Body, mapSelector.Body));
        return new QueryBuilder<TFirst, TSecond, TRelation, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TSecond, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, TRelation?>> mapSelector)
    {
        Handler.SetNext(new OneToOneJoinHandler<TSecond, TRelation, TReturn>(leftKeySelector.Body, rightKeySelector.Body, mapSelector.Body));
        return new QueryBuilder<TFirst, TSecond, TRelation, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TSecond, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, List<TRelation>?>> mapSelector)
    {
        Handler.SetNext(new OneToManyJoinHandler<TSecond, TRelation, TReturn>(leftKeySelector.Body, rightKeySelector.Body, mapSelector.Body));
        return new QueryBuilder<TFirst, TSecond, TRelation, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IGroupByBuilder<TFirst, TReturn> GroupBy(Expression<Func<TFirst, IComparable>> selector)
    {
        Handler.SetNext(new GroupByHandler(selector.Body));
        return new GroupQueryBuilder<TFirst, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TReturn> Where(Expression<Func<TFirst, bool>> predicate)
    {
        Handler.SetNext(new WhereHandler(predicate.Body));
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TReturn> Where(Expression<Func<TSecond, bool>> predicate)
    {
        Handler.SetNext(new WhereHandler(predicate.Body));
        return this;
    }

    /// <inheritdoc />
    public ISelectBuilder<TFirst, TSecond, TReturn> Select(Expression<Func<TFirst, TSecond, TReturn>> selector)
    {
        Handler.SetNext(new NewSelectHandler<TFirst, TReturn>(selector.Body));
        return this;
    }

    /// <inheritdoc />
    public IOrderByBuilder<TFirst, TSecond, TReturn> OrderBy<TKey>(Expression<Func<TFirst, TSecond, TKey>> selector)
        where TKey : IComparable<TKey>
    {
        Handler.SetNext(new OrderByHandler(selector.Body));
        return this;
    }

    /// <inheritdoc />
    public IOffsetBuilder<TFirst, TSecond, TReturn> Limit(int rows)
    {
        Handler.SetNext(new LimitHandler(rows));
        return this;
    }

    /// <inheritdoc />
    public ISqlBuilder<TFirst, TSecond, TReturn> Offset(int offset)
    {
        Handler.SetNext(new OffsetHandler(offset));
        return this;
    }

    /// <inheritdoc />
    public List<TReturn> ToList()
        => new CompositeQueryProxy<TReturn>().Create(Connection, Handler).GetList<TReturn>();
}

/// <summary>
///     Implements the query builder for three-table operations, providing methods
///     for constructing SQL queries that join three tables.
/// </summary>
/// <param name="Connection">The database connection used to execute queries.</param>
/// <param name="Handler">The query handler that manages query construction and execution.</param>
/// <typeparam name="TFirst">The type of the first table in the query.</typeparam>
/// <typeparam name="TSecond">The type of the second table in the query.</typeparam>
/// <typeparam name="TThird">The type of the third table in the query.</typeparam>
/// <typeparam name="TReturn">The type of the final query result.</typeparam>
public sealed record QueryBuilder<TFirst, TSecond, TThird, TReturn>(DbConnection Connection, QueryHandler Handler) :
    IQueryBuilder<TFirst, TSecond, TThird, TReturn>
{
    /// <inheritdoc />
    public IGroupByBuilder<TFirst, TReturn> GroupBy(Expression<Func<TFirst, IComparable?>> selector)
    {
        Handler.SetNext(new GroupByHandler(selector.Body));
        return new GroupQueryBuilder<TFirst, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TFirst, bool>> predicate)
    {
        Handler.SetNext(new WhereHandler(predicate.Body));
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TSecond, bool>> predicate)
    {
        Handler.SetNext(new WhereHandler(predicate.Body));
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TThird, bool>> predicate)
    {
        Handler.SetNext(new WhereHandler(predicate.Body));
        return this;
    }

    /// <inheritdoc />
    public ISelectBuilder<TFirst, TSecond, TThird, TReturn>
        Select(Expression<Func<TFirst, TSecond, TThird, TReturn>> selector)
    {
        Handler.SetNext(new NewSelectHandler<TFirst, TReturn>(selector.Body));
        return this;
    }

    /// <inheritdoc />
    public IOrderByBuilder<TFirst, TSecond, TThird, TReturn>
        OrderBy<TKey>(Expression<Func<TFirst, TSecond, TThird, TKey>> selector)
        where TKey : IComparable<TKey>
    {
        Handler.SetNext(new OrderByHandler(selector.Body));
        return this;
    }

    /// <inheritdoc />
    public IOffsetBuilder<TFirst, TSecond, TThird, TReturn> Limit(int rows)
    {
        Handler.SetNext(new LimitHandler(rows));
        return this;
    }

    /// <inheritdoc />
    public ISqlBuilder<TFirst, TSecond, TThird, TReturn> Offset(int offset)
    {
        Handler.SetNext(new OffsetHandler(offset));
        return this;
    }

    /// <inheritdoc />
    public List<TReturn> ToList()
        => new CompositeQueryProxy<TReturn>().Create(Connection, Handler).GetList<TReturn>();
}
