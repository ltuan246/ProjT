namespace KISS.QueryBuilder.Core;

/// <summary>
///     Contains the builder methods for different SQL clauses, which is probably how the query is constructed.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
public sealed record FluentSqlBuilder<TRecordset> : IQueryBuilder<TRecordset>
    where TRecordset : IEntityBuilder
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FluentSqlBuilder{TRecordset}" /> class.
    /// </summary>
    /// <param name="connection">
    ///     The <see cref="DbConnection" /> instance to be used for executing SQL queries.
    ///     This connection should be open and managed externally to ensure proper lifecycle handling.
    /// </param>
    public FluentSqlBuilder(DbConnection connection)
    {
        Connection = connection;

        SelectComponent selectComponent = new(SqlFormat, AliasMapping);
        QueryComponents[ClauseAction.Select] = [selectComponent];
    }

    private SqlFormatter SqlFormat { get; } = new();

    private Dictionary<Type, string> AliasMapping { get; } = [];

    private DbConnection Connection { get; }

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    private Dictionary<ClauseAction, List<IQueryComponent>> QueryComponents { get; init; } = new()
    {
        { ClauseAction.Select, [] },
        { ClauseAction.SelectFrom, [new SelectFromComponent(typeof(TRecordset))] },
        { ClauseAction.Join, [] },
        { ClauseAction.Where, [] },
        { ClauseAction.GroupBy, [] },
        { ClauseAction.Having, [] },
        { ClauseAction.OrderBy, [] },
        { ClauseAction.Limit, [] },
        { ClauseAction.Offset, [] }
    };

    /// <inheritdoc />
    public ISelectBuilder<TRecordset> Select(Expression<Func<TRecordset, object>> selector)
    {
        SelectComponent selectComponent = new(SqlFormat, AliasMapping);
        selectComponent.Selectors.Add(selector.Body);
        QueryComponents[ClauseAction.Select] = [selectComponent];
        return this;
    }

    /// <inheritdoc />
    public ISelectBuilder<TRecordset> SelectDistinct(Expression<Func<TRecordset, object>> selector)
    {
        SelectComponent selectComponent = new(SqlFormat, AliasMapping, true);
        selectComponent.Selectors.Add(selector.Body);
        QueryComponents[ClauseAction.Select] = [selectComponent];
        return Select(selector);
    }

    /// <inheritdoc />
    public IJoinBuilder<TRecordset> InnerJoin<TRelation, TKey>(
        Expression<Func<TRecordset, TRelation?>> mapSelector,
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>
        where TRelation : IEntityBuilder
    {
        JoinComponent component = new(typeof(TRelation), mapSelector.Body, leftKeySelector.Body, rightKeySelector.Body);
        QueryComponents[ClauseAction.Join].Add(component);
        return this;
    }

    /// <inheritdoc />
    public IJoinBuilder<TRecordset> InnerJoin<TRelation, TKey>(
        Expression<Func<TRecordset, List<TRelation>?>> mapSelector,
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>
        where TRelation : IEntityBuilder
    {
        JoinComponent component = new(typeof(TRelation), mapSelector.Body, leftKeySelector.Body, rightKeySelector.Body);
        QueryComponents[ClauseAction.Join].Add(component);
        return this;
    }

    /// <inheritdoc />
    public IJoinBuilder<TRecordset> InnerJoin<TRelation, TKey>(
        bool condition,
        Expression<Func<TRecordset, TRelation?>> mapSelector,
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>
        where TRelation : IEntityBuilder
        => condition ? InnerJoin(mapSelector, leftKeySelector, rightKeySelector) : this;

    /// <inheritdoc />
    public IJoinBuilder<TRecordset> InnerJoin<TRelation, TKey>(
        bool condition,
        Expression<Func<TRecordset, List<TRelation>?>> mapSelector,
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>
        where TRelation : IEntityBuilder
        => condition ? InnerJoin(mapSelector, leftKeySelector, rightKeySelector) : this;

    /// <inheritdoc />
    public IWhereBuilder<TRecordset> Where(Expression<Func<TRecordset, bool>> predicate)
    {
        WhereComponent component = new(SqlFormat, AliasMapping, predicate.Body);
        QueryComponents[ClauseAction.Where].Add(component);
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TRecordset> Where(bool condition, Expression<Func<TRecordset, bool>> predicate)
        => condition ? Where(predicate) : this;

    /// <inheritdoc />
    public IGroupByBuilder<TMap> GroupBy<TKey, TMap>(
        Expression<Func<TRecordset, TKey>> keySelector,
        Expression<Func<TKey, List<TRecordset>, TMap>> mapSelector)
        where TKey : IComparable<TKey>
        where TMap : IEntityBuilder
    {
        SelectComponent selectComponent = new(SqlFormat, AliasMapping);
        selectComponent.Selectors.Add(mapSelector.Body);
        QueryComponents[ClauseAction.Select] = [selectComponent];

        GroupByComponent groupByComponent = new(keySelector.Body);
        QueryComponents[ClauseAction.GroupBy] = [groupByComponent];

        IGroupByBuilder<TMap> builder = new FluentSqlBuilder<TMap>(Connection) { QueryComponents = QueryComponents };
        return builder;
    }

    /// <inheritdoc />
    public IAggregateBuilder<TRecordset> Aggregate() => this;

    /// <inheritdoc />
    public IHavingBuilder<TRecordset> Having() => this;

    /// <inheritdoc />
    public IHavingBuilder<TRecordset> Having(bool condition) => this;

    /// <inheritdoc />
    public IOrderByBuilder<TRecordset> OrderBy(Expression<Func<TRecordset, object>> selector) => this;

    /// <inheritdoc />
    public IOrderByBuilder<TRecordset> OrderBy(bool condition) => this;

    /// <inheritdoc />
    public IOffsetBuilder<TRecordset> Limit(int rows) => this;

    /// <inheritdoc />
    public IFluentSqlBuilder<TRecordset> Offset(int offset) => this;

    /// <inheritdoc />
    public List<TRecordset> ToList()
    {
        QueryVisitor visitor = new();

        foreach (var components in QueryComponents.Values)
        {
            foreach (var component in components)
            {
                component.Accept(visitor);
            }
        }

        // if (QueryComponents[ClauseAction.Join].Count != 0)
        // {
        //     Dictionary<string, TRecordset> dict = [];
        //     var map = visitor.BuildMapRecordset(dict);
        //     var query = visitor.CreatingQuery<TRecordset>();
        //     _ = query.Invoke(null, [
        //         Connection,
        //         visitor.Sql, // SQL query string
        //         map, // Mapping function
        //         visitor.Parameters, // Dapper DynamicParameters
        //         null, // IDbTransaction, set to null
        //         true, // Buffered, true by default
        //         "Id", // SplitOn, default to "Id"
        //         null, // CommandTimeout, null
        //         null // CommandType, null
        //     ]);
        //
        //     return dict.Values.ToList();
        // }

        return Connection.Query<TRecordset>(visitor.Sql, SqlFormat.Parameters).ToList();
    }
}
