namespace KISS.FluentSqlBuilder.Composites;

/// <summary>
///     A class that defines the fluent SQL builder type.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="CompositeQuery" /> class.
/// </remarks>
/// <param name="connection">The database connections.</param>
/// <param name="sourceEntity">The type representing the database record set.</param>
/// <param name="retrieveEntity">The combined type to return.</param>
public sealed partial class CompositeQuery(DbConnection connection, Type sourceEntity, Type retrieveEntity)
{
    /// <summary>
    ///     The database connections.
    /// </summary>
    private DbConnection Connection { get; } = connection;

    /// <summary>
    ///     The type representing the database record set.
    /// </summary>
    private Type SourceEntity { get; } = sourceEntity;

    /// <summary>
    ///     Used to represent an instance of the source entity in the expression tree.
    /// </summary>
    private ParameterExpression SourceParameter { get; } = Expression.Parameter(sourceEntity, "source");

    /// <summary>
    ///     The combined type to return.
    /// </summary>
    private Type RetrieveEntity { get; } = retrieveEntity;

    /// <summary>
    ///      Used to store the constructed target entity during the execution of the expression tree.
    /// </summary>
    private ParameterExpression RetrieveParameter { get; } = Expression.Variable(retrieveEntity, "retrieve");

    /// <summary>
    ///     The expressions are both ordered and intended for use in an Expression.Block.
    /// </summary>
    private List<(ParameterExpression Parameter, Expression Expr)> BlockMapSequence { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> SelectComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> SelectAsAliasComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<(SqlFunctions.AggregationType AggregationType, Expression Expr, string Alias)>
        SelectAggregationComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> SelectFromComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<(Type Recordset, Expression LeftKeySelector, Expression RightKeySelector)> JoinComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> WhereComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> GroupByComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> HavingComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> OrderByComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> LimitComponents { get; } = [];

    /// <summary>
    ///     Stores the query components categorized by clauses.
    /// </summary>
    public List<Expression> OffsetComponents { get; } = [];

    /// <summary>
    ///     Gets the query components.
    /// </summary>
    private void SetQueries()
    {
        SetSelect();
        SetFrom();
        SetJoin();
        SetWhere();
        SetGroupBy();
        SetHaving();
        SetOrderBy();
        SetLimit();
        SetOffset();
    }

    /// <summary>
    ///     Configures a mapping between a parent entity and a related entity.
    /// </summary>
    /// <param name="mapSelector">
    ///     An expression that specifies the property in the parent entity where the collection
    ///     of related entities is stored. This is typically a navigation property.
    /// </param>
    /// <typeparam name="TReturn">The type of the parent entity.</typeparam>
    /// <typeparam name="TRelation">The type of the related entities in the collection.</typeparam>
    /// <exception cref="NotSupportedException">
    ///     Thrown if the provided <paramref name="mapSelector" /> is not a valid member expression.
    /// </exception>
    public void SetMap<TReturn, TRelation>(Expression<Func<TReturn, TRelation?>> mapSelector)
    {
        switch (mapSelector.Body)
        {
            case MemberExpression memberExpression:
                {
                    var relationParam = Expression.Parameter(typeof(TRelation), GetAliasMapping(typeof(TRelation)));
                    var currentProperty = Expression.Property(RetrieveParameter, memberExpression.Member.Name);
                    var assignExpression = Expression.Assign(currentProperty, relationParam);
                    BlockMapSequence.Add((relationParam, assignExpression));

                    break;
                }

            default:
                throw new NotSupportedException("Expression not supported.");
        }
    }

    /// <summary>
    ///     Configures a mapping between a parent entity and a collection of related entities.
    /// </summary>
    /// <param name="mapSelector">
    ///     An expression that specifies the property in the parent entity where the collection
    ///     of related entities is stored. This is typically a navigation property.
    /// </param>
    /// <typeparam name="TReturn">The type of the parent entity.</typeparam>
    /// <typeparam name="TRelation">The type of the related entities in the collection.</typeparam>
    /// <exception cref="NotSupportedException">
    ///     Thrown if the provided <paramref name="mapSelector" /> is not a valid member expression.
    /// </exception>
    public void SetMap<TReturn, TRelation>(Expression<Func<TReturn, List<TRelation>?>> mapSelector)
    {
        switch (mapSelector.Body)
        {
            case MemberExpression memberExpression:
                {
                    var relationParam = Expression.Parameter(typeof(TRelation), GetAliasMapping(typeof(TRelation)));
                    var currentProperty = Expression.Property(RetrieveParameter, memberExpression.Member.Name);

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
    }

    /// <summary>
    ///     Creates a collection of <see cref="System.Linq.Expressions.MemberBinding" /> instances
    ///     that map properties from the source type to the target type.
    /// </summary>
    /// <returns>
    ///     An enumerable collection of <see cref="System.Linq.Expressions.MemberBinding" /> objects,
    ///     where each binding represents the assignment of a source property value to a target property.
    /// </returns>
    private IEnumerable<MemberBinding> CreateBindings()
    {
        var targetProperties = RetrieveEntity.GetProperties().Where(p => p.CanWrite).ToList();
        foreach (var targetProperty in targetProperties)
        {
            var sourceProperty = SourceEntity.GetProperty(targetProperty.Name);
            if (sourceProperty != null && sourceProperty.PropertyType == targetProperty.PropertyType)
            {
                var sourceValue = Expression.Property(SourceParameter, sourceProperty);
                var binding = Expression.Bind(targetProperty, sourceValue);
                yield return binding;
            }
        }
    }

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    public List<TReturn> ToList<TReturn>()
    {
        SetQueries();

        return JoinComponents.Count == 0 ? GetSingleMap<TReturn>() : GetMultiMap<TReturn>();
    }

    private List<TReturn> GetSingleMap<TReturn>()
    {
        if (SelectAggregationComponents.Count != 0)
        {
            return Connection
                .Query<dynamic, TReturn, TReturn>(
                    Sql,
                    (fst, scd) => scd,
                    Parameters)
                .ToList();
        }

        return Connection
            .Query<TReturn>(Sql, Parameters)
            .ToList();
    }

    private List<TReturn> GetMultiMap<TReturn>()
    {
        Dictionary<string, TReturn> dict = [];

        // Get the type of the dictionary parameter to use in reflection
        var dictType = dict.GetType();

        // Create a constant expression representing the dictionary, so it can be used in the expression tree
        var dictConst = Expression.Constant(dict);

        // Create a call to the dictionary's TryGetValue method to check if a record exists by its Id property
        var tryGetCall = Expression.Call(
            dictConst,
            dictType.GetMethod("TryGetValue")!,
            Expression.Property(SourceParameter, "Id"),
            RetrieveParameter);

        var initializer = Expression.MemberInit(Expression.New(RetrieveEntity), CreateBindings());

        // Define a block to add a new record to the dictionary if TryGetValue fails
        var addNewToDictBlock = Expression.Block(
            Expression.Assign(RetrieveParameter, initializer),
            Expression.Call(
                dictConst,
                dictType.GetMethod("Add")!,
                Expression.Property(SourceParameter, "Id"),
                RetrieveParameter));

        // If the record is not in the dictionary (TryGetValue returns false), add it using the block defined above
        var ifNotInDict = Expression.IfThen(Expression.IsFalse(tryGetCall), addNewToDictBlock);

        // Define parameters for the lambda expression
        var parameters = BlockMapSequence.Select(e => e.Parameter).ToList();
        var entities = parameters.Select(e => e.Type).ToList();

        // Define multiple expressions (Access method).
        var methods = BlockMapSequence.Select(e => e.Expr).ToList();

        // Combine expressions into a block: check dictionary, execute additional methods, and return result
        var block = Expression.Block([RetrieveParameter], [ifNotInDict, ..methods, RetrieveParameter]);

        // Create a list of parameter types for the query method
        List<Type> types = [SourceEntity, ..entities, RetrieveEntity];

        // Dynamically constructs the corresponding Func type using reflection.
        var funcType = typeof(Func<>).Assembly
            .GetType($"System.Func`{types.Count}")! // Func type has an additional return type
            .MakeGenericType([.. types]);

        // Create the lambda expression
        var lambda = Expression.Lambda(funcType, block, [SourceParameter, .. parameters]);
        var map = lambda.Compile();

        // Find a generic method named "Query" in SqlMapper with the correct number of generic arguments
        var queryMethod = typeof(SqlMapper)
            .GetMethods()
            .First(m => m is { Name: "Query", IsGenericMethod: true }
                        && m.GetGenericArguments().Length == types.Count);

        // Create a specialized generic method by applying types as generic parameters
        var query = queryMethod.MakeGenericMethod([.. types]);

        _ = query.Invoke(null, [
            Connection,
            Sql, // SQL query string
            map, // Mapping function
            Parameters, // Dapper DynamicParameters
            null, // IDbTransaction, set to null
            true, // Buffered, true by default
            "Id", // SplitOn, default to "Id", tells Dapper where the next object starts
            null, // CommandTimeout, null
            null // CommandType, null
        ]);

        return [.. dict.Values];
    }
}
