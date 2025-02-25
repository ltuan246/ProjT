namespace KISS.FluentSqlBuilder.QueryHandlerChain;

/// <summary>
///     Contains the builder methods for different SQL clauses, which is probably how the query is constructed.
/// </summary>
/// <param name="Connection">The database connections.</param>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
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
///     Contains the builder methods for different SQL clauses, which is probably how the query is constructed.
/// </summary>
/// <param name="Composite">Combining different queries together.</param>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record QueryBuilder<TRecordset, TReturn>(DbConnection Connection, QueryHandler Handler) :
    IQueryBuilder<TRecordset, TReturn>
{
    /// <inheritdoc />
    public IJoinBuilder<TRecordset, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TRecordset, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, TRelation?>> mapSelector)
    {
        Handler.SetNext(new JoinHandler(leftKeySelector, rightKeySelector, mapSelector));
        return new QueryBuilder<TRecordset, TRelation, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IJoinBuilder<TRecordset, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TRecordset, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, List<TRelation>?>> mapSelector)
    {
        Handler.SetNext(new JoinHandler(leftKeySelector, rightKeySelector, mapSelector));
        return new QueryBuilder<TRecordset, TRelation, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IWhereBuilder<TRecordset, TReturn> Where(Expression<Func<TRecordset, bool>> predicate)
    {
        Handler.SetNext(new WhereHandler(predicate));
        return this;
    }

    /// <inheritdoc />
    public IGroupByBuilder<TRecordset, TReturn> GroupBy(Expression<Func<TRecordset, IComparable>> selector)
    {
        Handler.SetNext(new GroupByHandler(selector));
        return new GroupQueryBuilder<TRecordset, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public ISelectBuilder<TRecordset, TReturn> Select(Expression<Func<TRecordset, TReturn>> selector)
    {
        Handler.SetNext(new NewSelectHandler<TRecordset, TReturn>(selector));
        return this;
    }

    /// <inheritdoc />
    public IOrderByBuilder<TRecordset, TReturn> OrderBy<TKey>(Expression<Func<TRecordset, TKey>> selector)
        where TKey : IComparable<TKey>
    {
        Handler.SetNext(new OrderByHandler(selector));
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
    {
        var composite = new CompositeQuery(Connection);
        Handler.Handle(composite);
        return [];
    }
}

/// <summary>
///     Contains the builder methods for different SQL clauses, which is probably how the query is constructed.
/// </summary>
/// <param name="Composite">Combining different queries together.</param>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record GroupQueryBuilder<TRecordset, TReturn>(DbConnection Connection, QueryHandler Handler) :
    IGroupQueryBuilder<TRecordset, TReturn>
{
    /// <inheritdoc />
    public IGroupByBuilder<TRecordset, TReturn> ThenBy(Expression<Func<TRecordset, IComparable>> selector)
    {
        Handler.SetNext(new GroupByHandler(selector));
        return this;
    }

    /// <inheritdoc />
    public IHavingBuilder<TRecordset, TReturn> Having(Expression<Func<TRecordset, IComparable>> selector)
    {
        Handler.SetNext(new HavingHandler(selector));
        return this;
    }

    /// <inheritdoc />
    public IGroupSelectBuilder<TRecordset, TReturn> Select(
        SqlFunctions.AggregationType aggregationType,
        Expression<Func<TRecordset, IComparable>> selector,
        string alias) =>
        this;

    /// <inheritdoc />
    public IGroupSelectBuilder<TRecordset, TReturn> Select(Expression<Func<TRecordset, TReturn>> selector)
    {
        Handler.SetNext(new NewSelectHandler<TRecordset, TReturn>(selector));
        return this;
    }

    /// <inheritdoc />
    public IGroupOrderByBuilder<TRecordset, TReturn> OrderBy<TKey>(Expression<Func<TRecordset, TKey>> selector)
        where TKey : IComparable<TKey>
    {
        Handler.SetNext(new OrderByHandler(selector));
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
    public List<TReturn> ToGroupList()
    {
        var composite = new CompositeQuery(Connection);
        Handler.Handle(composite);
        return [];
    }
}

/// <summary>
///     Contains the builder methods for different SQL clauses, which is probably how the query is constructed.
/// </summary>
/// <param name="Composite">Combining different queries together.</param>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record QueryBuilder<TFirst, TSecond, TReturn>(DbConnection Connection, QueryHandler Handler) :
    IQueryBuilder<TFirst, TSecond, TReturn>
{
    /// <inheritdoc />
    public IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TFirst, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, TRelation?>> mapSelector)
    {
        Handler.SetNext(new JoinHandler(leftKeySelector, rightKeySelector, mapSelector));
        return new QueryBuilder<TFirst, TSecond, TRelation, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TFirst, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, List<TRelation>?>> mapSelector)
    {
        Handler.SetNext(new JoinHandler(leftKeySelector, rightKeySelector, mapSelector));
        return new QueryBuilder<TFirst, TSecond, TRelation, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TSecond, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, TRelation?>> mapSelector)
    {
        Handler.SetNext(new JoinHandler(leftKeySelector, rightKeySelector, mapSelector));
        return new QueryBuilder<TFirst, TSecond, TRelation, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation>(
        Expression<Func<TSecond, IComparable>> leftKeySelector,
        Expression<Func<TRelation, IComparable>> rightKeySelector,
        Expression<Func<TReturn, List<TRelation>?>> mapSelector)
    {
        Handler.SetNext(new JoinHandler(leftKeySelector, rightKeySelector, mapSelector));
        return new QueryBuilder<TFirst, TSecond, TRelation, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IGroupByBuilder<TFirst, TReturn> GroupBy(Expression<Func<TFirst, IComparable>> selector)
    {
        Handler.SetNext(new GroupByHandler(selector));
        return new GroupQueryBuilder<TFirst, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TReturn> Where(Expression<Func<TFirst, bool>> predicate)
    {
        Handler.SetNext(new WhereHandler(predicate));
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TReturn> Where(Expression<Func<TSecond, bool>> predicate)
    {
        Handler.SetNext(new WhereHandler(predicate));
        return this;
    }

    /// <inheritdoc />
    public ISelectBuilder<TFirst, TSecond, TReturn> Select(Expression<Func<TFirst, TSecond, TReturn>> selector)
    {
        Handler.SetNext(new NewSelectHandler<TFirst, TReturn>(selector));
        return this;
    }

    /// <inheritdoc />
    public IOrderByBuilder<TFirst, TSecond, TReturn> OrderBy<TKey>(Expression<Func<TFirst, TSecond, TKey>> selector)
        where TKey : IComparable<TKey>
    {
        Handler.SetNext(new OrderByHandler(selector));
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
    public List<TReturn> ToList() => [];
}

/// <summary>
///     Contains the builder methods for different SQL clauses, which is probably how the query is constructed.
/// </summary>
/// <param name="Composite">Combining different queries together.</param>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record QueryBuilder<TFirst, TSecond, TThird, TReturn>(DbConnection Connection, QueryHandler Handler) :
    IQueryBuilder<TFirst, TSecond, TThird, TReturn>
{
    /// <inheritdoc />
    public IGroupByBuilder<TFirst, TReturn> GroupBy(Expression<Func<TFirst, IComparable>> selector)
    {
        Handler.SetNext(new GroupByHandler(selector));
        return new GroupQueryBuilder<TFirst, TReturn>(Connection, Handler);
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TFirst, bool>> predicate)
    {
        Handler.SetNext(new WhereHandler(predicate));
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TSecond, bool>> predicate)
    {
        Handler.SetNext(new WhereHandler(predicate));
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TThird, bool>> predicate)
    {
        Handler.SetNext(new WhereHandler(predicate));
        return this;
    }

    /// <inheritdoc />
    public ISelectBuilder<TFirst, TSecond, TThird, TReturn>
        Select(Expression<Func<TFirst, TSecond, TThird, TReturn>> selector)
    {
        Handler.SetNext(new NewSelectHandler<TFirst, TReturn>(selector));
        return this;
    }

    /// <inheritdoc />
    public IOrderByBuilder<TFirst, TSecond, TThird, TReturn>
        OrderBy<TKey>(Expression<Func<TFirst, TSecond, TThird, TKey>> selector)
        where TKey : IComparable<TKey>
    {
        Handler.SetNext(new OrderByHandler(selector));
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
    public List<TReturn> ToList() => [];
}
