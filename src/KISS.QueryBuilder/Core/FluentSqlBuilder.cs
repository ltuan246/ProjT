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
        AliasMapping = new() { { typeof(TRecordset), $"{ClauseConstants.DefaultTableAlias}{0}" } };
        SelectClause = new(SqlFormat, AliasMapping);
        JoinClause = new(AliasMapping);
        WhereClause = new(SqlFormat, AliasMapping);
        OrderByClause = new(AliasMapping);
        LimitClause = new();
        OffsetClause = new();
        QueryComponents =
        [
            SelectClause,
            new SelectFromComponent(typeof(TRecordset)),
            JoinClause,
            WhereClause,
            OrderByClause,
            LimitClause,
            OffsetClause
        ];
    }

    private SqlFormatter SqlFormat { get; } = new();

    private Dictionary<Type, string> AliasMapping { get; }

    private DbConnection Connection { get; }

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    private List<IQueryComponent> QueryComponents { get; }

    /// <summary>
    ///     The expressions are both ordered and intended for use in an Expression.Block.
    /// </summary>
    private List<(ParameterExpression Parameter, Expression Expr)> BlockMapSequence { get; } = [];

    private ParameterExpression ReturnParam { get; } = Expression.Variable(typeof(TRecordset), "currentRecordset");

    // private Dictionary<ClauseAction, List<IQueryComponent>> QueryComponents { get; init; } = new()
    // {
    //     { ClauseAction.Select, [] },
    //     { ClauseAction.SelectFrom, [new SelectFromComponent(typeof(TRecordset))] },
    //     { ClauseAction.Join, [] },
    //     { ClauseAction.Where, [] },
    //     { ClauseAction.GroupBy, [] },
    //     { ClauseAction.Having, [] },
    //     { ClauseAction.OrderBy, [] },
    //     { ClauseAction.Limit, [] },
    //     { ClauseAction.Offset, [] }
    // };

    private SelectComponent SelectClause { get; }
    private JoinComponent JoinClause { get; }
    private WhereComponent WhereClause { get; }
    private OrderByComponent OrderByClause { get; }
    private LimitComponent LimitClause { get; }
    private OffsetComponent OffsetClause { get; }

    /// <inheritdoc />
    public ISelectBuilder<TRecordset> Select(Expression<Func<TRecordset, object>> selector)
    {
        SelectClause.Selectors.Clear();
        SelectClause.Selectors.Add(selector.Body);
        return this;
    }

    /// <inheritdoc />
    public ISelectBuilder<TRecordset> SelectDistinct(Expression<Func<TRecordset, object>> selector)
    {
        SelectClause.HasDistinct = true;
        SelectClause.Selectors.Clear();
        SelectClause.Selectors.Add(selector.Body);
        return this;
    }

    /// <inheritdoc />
    public IJoinBuilder<TRecordset> InnerJoin<TRelation, TKey>(
        Expression<Func<TRecordset, TRelation?>> mapSelector,
        Expression<Func<TRecordset, TKey>> leftKeySelector,
        Expression<Func<TRelation, TKey>> rightKeySelector)
        where TKey : IComparable<TKey>
        where TRelation : IEntityBuilder
    {
        switch (mapSelector.Body)
        {
            case MemberExpression memberExpression:
                {
                    var relationParam = Expression.Parameter(typeof(TRelation), GetAliasMapping(typeof(TRelation)));
                    var currentProperty = Expression.Property(ReturnParam, memberExpression.Member.Name);
                    var assignExpression = Expression.Assign(currentProperty, relationParam);
                    BlockMapSequence.Add((relationParam, assignExpression));

                    break;
                }

            default:
                throw new NotSupportedException("Expression not supported.");
        }

        if (SelectClause.Selectors.Count != 0)
        {
            Expression ex = Expression.Constant($"{GetAliasMapping(typeof(TRelation))}.*");
            SelectClause.Selectors.Add(ex);
        }

        JoinClause.Joins.Add((typeof(TRelation), leftKeySelector.Body, rightKeySelector.Body));

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
        switch (mapSelector.Body)
        {
            case MemberExpression memberExpression:
                {
                    var relationParam = Expression.Parameter(typeof(TRelation), GetAliasMapping(typeof(TRelation)));
                    var currentProperty = Expression.Property(ReturnParam, memberExpression.Member.Name);

                    var newList = Expression.IfThen(
                        Expression.Equal(currentProperty, Expression.Constant(null)),
                        Expression.Assign(currentProperty, Expression.New(typeof(List<TRelation>))));

                    // Add relation to mapSelector if it exists
                    var addToList = Expression.IfThen(
                        Expression.NotEqual(relationParam, Expression.Constant(null)),
                        Expression.Block(
                            newList,
                            Expression.Call(currentProperty, "Add", null, relationParam)));

                    BlockMapSequence.Add((relationParam, addToList));

                    break;
                }

            default:
                throw new NotSupportedException("Expression not supported.");
        }

        if (SelectClause.Selectors.Count != 0)
        {
            Expression ex = Expression.Constant($"{GetAliasMapping(typeof(TRelation))}.*");
            SelectClause.Selectors.Add(ex);
        }

        JoinClause.Joins.Add((typeof(TRelation), leftKeySelector.Body, rightKeySelector.Body));

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
        WhereClause.Predicate.Add(predicate.Body);
        return this;
    }

    /// <inheritdoc />
    public IWhereBuilder<TRecordset> Where(bool condition, Expression<Func<TRecordset, bool>> predicate)
        => condition ? Where(predicate) : this;

    /// <inheritdoc />
    public IGroupByBuilder<TRecordset> GroupBy<TKey, TMap>(
        Expression<Func<TRecordset, TKey>> keySelector,
        Expression<Func<TKey, List<TRecordset>, TMap>> mapSelector)
        where TKey : IComparable<TKey>
        where TMap : IEntityBuilder
    {
        _ = new GroupByComponent(keySelector.Body);
        return this;
    }

    /// <inheritdoc />
    public IAggregateBuilder<TRecordset> Aggregate() => this;

    /// <inheritdoc />
    public IHavingBuilder<TRecordset> Having() => this;

    /// <inheritdoc />
    public IHavingBuilder<TRecordset> Having(bool condition) => this;

    /// <inheritdoc />
    public IOrderByBuilder<TRecordset> OrderBy(Expression<Func<TRecordset, object>> selector)
    {
        OrderByClause.Selectors.Add(selector.Body);
        return this;
    }

    /// <inheritdoc />
    public IOrderByBuilder<TRecordset> OrderBy(bool condition) => this;

    /// <inheritdoc />
    public IOffsetBuilder<TRecordset> Limit(int rows)
    {
        LimitClause.Rows = rows;
        return this;
    }

    /// <inheritdoc />
    public IFluentSqlBuilder<TRecordset> Offset(int offset)
    {
        OffsetClause.Offset = offset;
        return this;
    }

    /// <inheritdoc />
    public List<TRecordset> ToList()
    {
        QueryVisitor visitor = new();

        foreach (var component in QueryComponents)
        {
            component.Accept(visitor);
        }

        if (JoinClause.Joins.Count != 0)
        {
            Dictionary<string, TRecordset> dict = [];
            var map = BuildMapRecordset(dict);
            var query = CreatingQuery();
            _ = query.Invoke(null, [
                Connection,
                visitor.Sql, // SQL query string
                map, // Mapping function
                SqlFormat.Parameters, // Dapper DynamicParameters
                null, // IDbTransaction, set to null
                true, // Buffered, true by default
                "Id", // SplitOn, default to "Id"
                null, // CommandTimeout, null
                null // CommandType, null
            ]);

            return dict.Values.ToList();
        }

        return Connection.Query<TRecordset>(visitor.Sql, SqlFormat.Parameters).ToList();
    }

    /// <summary>
    ///     Generates a delegate that maps records into a dictionary based on a unique identifier (e.g., `Id`).
    ///     If a record with the given identifier does not already exist in the dictionary, it will be added.
    ///     This delegate is later used by Dapper as a custom mapping function for database records.
    /// </summary>
    /// <param name="dict">A dictionary used to store and retrieve records of type `TRecordset` by ID.</param>
    /// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
    /// <returns>A compiled delegate (Func) that performs dictionary lookups and conditionally adds new records.</returns>
    private Delegate BuildMapRecordset(Dictionary<string, TRecordset> dict)
    {
        // Get the type of the dictionary parameter to use in reflection
        var dictType = dict.GetType();

        // Define parameters for the lambda expression
        var parameters = BlockMapSequence.Select(e => e.Parameter).ToList();

        // Define multiple expressions (Access method).
        var methods = BlockMapSequence.Select(e => e.Expr).ToList();

        // Create a constant expression representing the dictionary, so it can be used in the expression tree
        var dictParam = Expression.Constant(dict);

        // Define a parameter expression for the lambda function, representing a single record of type TRecordset
        var recordsetParam = Expression.Parameter(typeof(TRecordset), "recordset");

        // Create a call to the dictionary's TryGetValue method to check if a record exists by its Id property
        var tryGetValueCall = Expression.Call(
            dictParam,
            dictType.GetMethod("TryGetValue")!,
            Expression.Property(recordsetParam, "Id"),
            ReturnParam);

        // Define a block to add a new record to the dictionary if TryGetValue fails
        var addNewToDictBlock = Expression.Block(
            Expression.Assign(ReturnParam, recordsetParam),
            Expression.Call(
                dictParam,
                dictType.GetMethod("Add")!,
                Expression.Property(recordsetParam, "Id"),
                recordsetParam));

        // If the record is not in the dictionary (TryGetValue returns false), add it using the block defined above
        var ifNotInDict = Expression.IfThen(Expression.IsFalse(tryGetValueCall), addNewToDictBlock);

        // Combine expressions into a block: check dictionary, execute additional methods, and return result
        var block = Expression.Block([ReturnParam], [ifNotInDict, .. methods, ReturnParam]);

        // Dynamically constructs the corresponding Func type using reflection.
        List<Type> types = [.. AliasMapping.Keys, typeof(TRecordset)];
        var funcType = typeof(Func<>).Assembly
            .GetType($"System.Func`{types.Count}")! // Func type has an additional return type
            .MakeGenericType([.. types]);

        // Create the lambda expression
        var lambda = Expression.Lambda(funcType, block, [recordsetParam, .. parameters]);

        // Compile the expression tree into a delegate
        var map = lambda.Compile();

        return map;
    }

    /// <summary>
    ///     Locates the appropriate generic `Query` method from Dapper's `SqlMapper` type, which allows custom mapping.
    ///     Constructs a version of this method with the correct generic type arguments based on `TRecordset` and
    ///     additional types provided by `TableAliasesMap`.
    /// </summary>
    /// <typeparam name="TRecordset">The type representing the database record set.</typeparam>
    /// <returns>A MethodInfo object representing the generic `Query` method tailored for the type mappings.</returns>
    private MethodInfo CreatingQuery()
    {
        // Create a list of parameter types for the query method
        List<Type> types = [.. AliasMapping.Keys, typeof(TRecordset)];

        // Find a generic method named "Query" in SqlMapper with the correct number of generic arguments
        var queryMethod = typeof(SqlMapper)
            .GetMethods()
            .First(m => m is { Name: "Query", IsGenericMethod: true }
                        && m.GetGenericArguments().Length == types.Count);

        // Create a specialized generic method by applying types as generic parameters
        var method = queryMethod.MakeGenericMethod([.. types]);

        return method;
    }

    /// <summary>
    ///     Retrieves the alias mapped to the specified <see cref="Type" /> in the query context,
    ///     or creates a new alias if none exists.
    /// </summary>
    /// <param name="type">The <see cref="Type" /> for which to retrieve or generate a table alias.</param>
    /// <returns>
    ///     A <see cref="string" /> representing the alias associated with the specified <paramref name="type" />.
    /// </returns>
    private string GetAliasMapping(Type type)
    {
        if (!AliasMapping.TryGetValue(type, out var tableAlias))
        {
            // Generate a new alias based on the default alias prefix and current alias count.
            tableAlias = $"{ClauseConstants.DefaultTableAlias}{AliasMapping.Count}";

            // Store the new alias in the dictionary for future reference.
            AliasMapping.Add(type, tableAlias);
        }

        // Return the alias (existing or newly created) for the specified type.
        return tableAlias;
    }
}
