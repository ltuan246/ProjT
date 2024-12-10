namespace KISS.FluentSqlBuilder.Core;

/// <summary>
///     Contains the builder methods for different SQL clauses, which is probably how the query is constructed.
/// </summary>
/// <param name="Composite">Combining different queries together.</param>
/// <param name="Connection">The database connections.</param>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record QueryBuilder<TReturn>(CompositeQuery Composite, DbConnection Connection) :
    IQueryBuilderEntry<TReturn>
{
    /// <inheritdoc />
    public IJoinBuilder<TRecordset, TReturn> From<TRecordset>()
    {
        ConstantExpression constantExpression = Expression.Constant(typeof(TRecordset));
        Composite.SelectFromComponents.Add(constantExpression);
        return new QueryBuilder<TRecordset, TReturn>(Composite, Connection);
    }
}

/// <summary>
///     Contains the builder methods for different SQL clauses, which is probably how the query is constructed.
/// </summary>
/// <param name="Composite">Combining different queries together.</param>
/// <param name="Connection">The database connections.</param>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record QueryBuilder<TRecordset, TReturn>(CompositeQuery Composite, DbConnection Connection) :
    IQueryBuilder<TRecordset, TReturn>
{
    /// <inheritdoc />
    public IJoinBuilder<TRecordset, TRelation, TReturn> InnerJoin<TRelation, TKey>(
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>
    {
        Composite.JoinComponents.Add((typeof(TRelation), leftKeySelector.Body, rightKeySelector.Body));
        return new QueryBuilder<TRecordset, TRelation, TReturn>(Composite, Connection);
    }

    /// <inheritdoc />
    public IWhereBuilder<TRecordset, TReturn> Where(Expression<Func<TRecordset, bool>> predicate)
    {
        Composite.WhereComponents.Add(predicate.Body);
        return this;
    }

    /// <inheritdoc />
    public ISelectBuilder<TRecordset, TReturn> Select(Expression<Func<TRecordset, TReturn>> selector)
    {
        Composite.SelectComponents.Add(selector.Body);
        return this;
    }

    /// <inheritdoc />
    public IOrderByBuilder<TRecordset, TReturn> OrderBy<TKey>(Expression<Func<TRecordset, TKey>> selector)
        where TKey : IComparable<TKey>
    {
        Composite.OrderByComponents.Add(selector.Body);
        return this;
    }

    /// <inheritdoc />
    public IOffsetBuilder<TRecordset, TReturn> Limit(int rows)
    {
        ConstantExpression constantExpression = Expression.Constant(rows);
        Composite.LimitComponents.Add(constantExpression);
        return this;
    }

    /// <inheritdoc />
    public ISqlBuilder<TRecordset, TReturn> Offset(int offset)
    {
        ConstantExpression constantExpression = Expression.Constant(offset);
        Composite.OffsetComponents.Add(constantExpression);
        return this;
    }

    /// <inheritdoc />
    public List<TReturn> ToList()
    {
        Composite.SetQueries();
        return Connection.Query<TReturn>(Composite.Sql, Composite.Parameters).ToList();
    }
}

/// <summary>
///     Contains the builder methods for different SQL clauses, which is probably how the query is constructed.
/// </summary>
/// <param name="Composite">Combining different queries together.</param>
/// <param name="Connection">The database connections.</param>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record QueryBuilder<TFirst, TSecond, TReturn>(CompositeQuery Composite, DbConnection Connection) :
    IQueryBuilder<TFirst, TSecond, TReturn>
{
    /// <inheritdoc />
    public IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation, TKey>(
        Expression<Func<TFirst, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>
    {
        Composite.JoinComponents.Add((typeof(TRelation), leftKeySelector.Body, rightKeySelector.Body));
        return new QueryBuilder<TFirst, TSecond, TRelation, TReturn>(Composite, Connection);
    }

    /// <inheritdoc />
    public IJoinBuilder<TFirst, TSecond, TRelation, TReturn> InnerJoin<TRelation, TKey>(
        Expression<Func<TSecond, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>
    {
        Composite.JoinComponents.Add((typeof(TRelation), leftKeySelector.Body, rightKeySelector.Body));
        return new QueryBuilder<TFirst, TSecond, TRelation, TReturn>(Composite, Connection);
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TReturn> Where(Expression<Func<TFirst, bool>> predicate)
    {
        Composite.WhereComponents.Add(predicate.Body);
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TReturn> Where(Expression<Func<TSecond, bool>> predicate)
    {
        Composite.WhereComponents.Add(predicate.Body);
        return this;
    }

    /// <inheritdoc />
    public ISelectBuilder<TFirst, TSecond, TReturn> Select(Expression<Func<TFirst, TSecond, TReturn>> selector)
    {
        Composite.SelectComponents.Add(selector.Body);
        return this;
    }

    /// <inheritdoc />
    public IOrderByBuilder<TFirst, TSecond, TReturn> OrderBy<TKey>(Expression<Func<TFirst, TSecond, TKey>> selector)
        where TKey : IComparable<TKey>
    {
        Composite.OrderByComponents.Add(selector.Body);
        return this;
    }

    /// <inheritdoc />
    public IOffsetBuilder<TFirst, TSecond, TReturn> Limit(int rows)
    {
        ConstantExpression constantExpression = Expression.Constant(rows);
        Composite.LimitComponents.Add(constantExpression);
        return this;
    }

    /// <inheritdoc />
    public ISqlBuilder<TFirst, TSecond, TReturn> Offset(int offset)
    {
        ConstantExpression constantExpression = Expression.Constant(offset);
        Composite.OffsetComponents.Add(constantExpression);
        return this;
    }

    /// <inheritdoc />
    public List<TReturn> ToList()
    {
        Composite.SetQueries();
        return Connection.Query<TReturn>(Composite.Sql, Composite.Parameters).ToList();
    }
}

/// <summary>
///     Contains the builder methods for different SQL clauses, which is probably how the query is constructed.
/// </summary>
/// <param name="Composite">Combining different queries together.</param>
/// <param name="Connection">The database connections.</param>
/// <typeparam name="TFirst">The first type in the recordset.</typeparam>
/// <typeparam name="TSecond">The second type in the recordset.</typeparam>
/// <typeparam name="TThird">The third type in the recordset.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed record QueryBuilder<TFirst, TSecond, TThird, TReturn>(CompositeQuery Composite, DbConnection Connection) :
    IQueryBuilder<TFirst, TSecond, TThird, TReturn>
{
    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TFirst, bool>> predicate)
    {
        Composite.WhereComponents.Add(predicate.Body);
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TSecond, bool>> predicate)
    {
        Composite.WhereComponents.Add(predicate.Body);
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TFirst, TSecond, TThird, TReturn> Where(Expression<Func<TThird, bool>> predicate)
    {
        Composite.WhereComponents.Add(predicate.Body);
        return this;
    }

    /// <inheritdoc />
    public ISelectBuilder<TFirst, TSecond, TThird, TReturn>
        Select(Expression<Func<TFirst, TSecond, TThird, TReturn>> selector)
    {
        Composite.SelectComponents.Add(selector.Body);
        return this;
    }

    /// <inheritdoc />
    public IOrderByBuilder<TFirst, TSecond, TThird, TReturn>
        OrderBy<TKey>(Expression<Func<TFirst, TSecond, TThird, TKey>> selector)
        where TKey : IComparable<TKey>
    {
        Composite.OrderByComponents.Add(selector.Body);
        return this;
    }

    /// <inheritdoc />
    public IOffsetBuilder<TFirst, TSecond, TThird, TReturn> Limit(int rows)
    {
        ConstantExpression constantExpression = Expression.Constant(rows);
        Composite.LimitComponents.Add(constantExpression);
        return this;
    }

    /// <inheritdoc />
    public ISqlBuilder<TFirst, TSecond, TThird, TReturn> Offset(int offset)
    {
        ConstantExpression constantExpression = Expression.Constant(offset);
        Composite.OffsetComponents.Add(constantExpression);
        return this;
    }

    /// <inheritdoc />
    public List<TReturn> ToList()
    {
        Composite.SetQueries();
        return Connection
            .Query<TReturn, TSecond, TThird, TReturn>(
                Composite.Sql,
                (order, customer, product) => order,
                Composite.Parameters,
                splitOn: "Id,Id,Id")
            .ToList();
    }
}
