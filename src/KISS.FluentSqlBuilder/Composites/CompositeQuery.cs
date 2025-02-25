namespace KISS.FluentSqlBuilder.Composites;

/// <summary>
///     A class that defines the fluent SQL builder type.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="CompositeQuery{TSource, TReturn}" /> class.
/// </remarks>
/// <param name="connection">The database connections.</param>
/// <typeparam name="TSource">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed partial class CompositeQuery<TSource, TReturn>(DbConnection connection) : ICompositeQuery, IDataRetrieval<TReturn>
{
    /// <summary>
    ///     The type representing the database record set.
    /// </summary>
    public Type SourceEntity { get; } = typeof(TSource);

    /// <summary>
    ///     The combined type to return.
    /// </summary>
    public Type RetrieveEntity { get; } = typeof(TReturn);

    /// <summary>
    ///     The database connections.
    /// </summary>
    private DbConnection Connection { get; } = connection;

    /// <summary>
    ///     Used to represent an instance of the source entity in the expression tree.
    /// </summary>
    private ParameterExpression SourceParameter { get; } = Expression.Parameter(typeof(TSource), "source");

    /// <summary>
    ///      Used to store the constructed target entity during the execution of the expression tree.
    /// </summary>
    private ParameterExpression RetrieveVariable { get; } = Expression.Variable(typeof(TReturn), "retrieve");

    /// <summary>
    ///     The expressions are both ordered and intended for use in an Expression.Block.
    /// </summary>
    private List<(ParameterExpression Parameter, Expression Expr)> BlockMapSequence { get; } = [];

    /// <summary>
    ///     Stores the mapping of properties.
    /// </summary>
    public Dictionary<Type, (string, PropertyInfo[])> MapProfiles { get; } = [];

    /// <summary>
    ///     Stores the group key.
    /// </summary>
    public List<(string Alias, string Key, string GroupKey)> GroupKeys { get; } = [];

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
        SelectAggregationComponents
    { get; } = [];

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

    private static Type DtRowType { get; } = typeof(IDictionary<string, object>);

    private static ParameterExpression DtRowParam { get; } = Expression.Parameter(DtRowType, "dict");

    /// <summary>
    ///     Retrieves the MethodInfo for IEnumerator.MoveNext(), which advances the enumerator
    ///     to the next element in the sequence. Used for controlling iteration in expression trees.
    /// </summary>
    private static MethodInfo StreamMoveNextMethod { get; } = typeof(IEnumerator).GetMethod("MoveNext")!;

    /// <summary>
    ///     A single row in the result set as a dictionary of column names and values.
    /// </summary>
    private static Type DapperRowType { get; } = typeof(IDictionary<string, object>);

    /// <summary>
    ///     A collection of rows, allowing iteration over multiple dictionaries.
    /// </summary>
    private static Type DapperRowCollectionType { get; } = typeof(IEnumerable<IDictionary<string, object>>);

    /// <summary>
    ///     An enumerator for iterating over rows one at a time, typically used for streaming data.
    /// </summary>
    private static Type DapperRowStreamType { get; } = typeof(IEnumerator<IDictionary<string, object>>);

    /// <summary>
    ///     Retrieves the MethodInfo for GetEnumerator() on a Dapper row collection type.
    ///     This method is used to obtain an enumerator that enables iteration over the query results.
    /// </summary>
    private MethodInfo DapperRowStreamMethod { get; } = DapperRowCollectionType.GetMethod("GetEnumerator")!;

    private ParameterExpression DapperRowCollectionParameter { get; } = Expression.Parameter(DapperRowCollectionType, "dataCollection");

    private ParameterExpression GroupingDapperRowParameter { get; } = Expression.Parameter(typeof(Dictionary<ITuple, Dictionary<string, TReturn>>), "retrieves");

    /// <summary>
    ///     Stores the group key.
    /// </summary>
    public ParameterExpression DapperRowVariable { get; } = Expression.Variable(DapperRowType, "dataRow");

    private ParameterExpression DapperRowStreamVariable { get; } = Expression.Variable(DapperRowStreamType, "dataStream");

    /// <summary>
    ///     Variables that are used within the while-loop to facilitate row handling and processing.
    /// </summary>
    // private List<ParameterExpression> SequenceRowHandlingVariables { get; } = [];

    /// <summary>
    ///     A sequence of stepwise operations for processing each data row inside the while loop.
    /// </summary>
    private List<Expression> SequenceDapperRowProcessing { get; } = [];

    /// <summary>
    ///     Stores the group key.
    /// </summary>
    public List<MemberAssignment> RetrievePropertyAssignmentProcessing { get; } = [];

    /// <summary>
    ///     Stores the group key.
    /// </summary>
    public List<(Type GroupingKeyType, Expression PropertyAssignment)> GroupingPropertyAssignmentProcessing { get; } = [];

    /// <summary>
    ///     Creates an expression that converts a given property expression to a specified target type
    ///     using Convert.ChangeType(). This ensures dynamic type conversion at runtime within an expression tree.
    /// </summary>
    /// <param name="targetProperty">The expression representing the property to convert.</param>
    /// <param name="targetType">The target type to which the property should be converted.</param>
    /// <returns>A UnaryExpression that represents the type conversion.</returns>
    public UnaryExpression ChangeType(Expression targetProperty, Type targetType) =>
        Expression.Convert(Expression.Call(typeof(Convert), nameof(Convert.ChangeType), [], targetProperty, Expression.Constant(targetType)), targetType);

    /// <summary>
    ///     Gets the query components.
    /// </summary>
    public void SetQueries()
    {
        SetSelect();
        SetFrom();
        SetJoin();
        SetWhere();
        SetGroupBy();
        // SetHaving();
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
    /// <typeparam name="TRelation">The type of the related entities in the collection.</typeparam>
    /// <exception cref="NotSupportedException">
    ///     Thrown if the provided <paramref name="mapSelector" /> is not a valid member expression.
    /// </exception>
    public void SetMap<TRelation>(Expression<Func<TReturn, TRelation?>> mapSelector)
    {
        switch (mapSelector.Body)
        {
            case MemberExpression memberExpression:
                {
                    var relationParam = Expression.Parameter(typeof(TRelation), GetAliasMapping(typeof(TRelation)));
                    var currentProperty = Expression.Property(RetrieveVariable, memberExpression.Member.Name);
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
    /// <typeparam name="TRelation">The type of the related entities in the collection.</typeparam>
    /// <exception cref="NotSupportedException">
    ///     Thrown if the provided <paramref name="mapSelector" /> is not a valid member expression.
    /// </exception>
    public void SetMap<TRelation>(Expression<Func<TReturn, List<TRelation>?>> mapSelector)
    {
        switch (mapSelector.Body)
        {
            case MemberExpression memberExpression:
                {
                    var relationParam = Expression.Parameter(typeof(TRelation), GetAliasMapping(typeof(TRelation)));
                    var currentProperty = Expression.Property(RetrieveVariable, memberExpression.Member.Name);

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
    ///     Creates a collection of <see cref="MemberBinding" /> instances
    ///     that map properties from the source type to the target type.
    /// </summary>
    /// <returns>
    ///     An enumerable collection of <see cref="MemberBinding" /> objects,
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

    private IEnumerable<MemberBinding> GroupCreateBindings(string alias, PropertyInfo[] targetProperties)
    {
        foreach (var targetProperty in targetProperties)
        {
            var sourceValue = Expression.Property(DtRowParam, "Item", Expression.Constant($"{alias}_{targetProperty.Name}"));

            // If the target type is nullable (e.g., Nullable<T>), retrieve its underlying non-nullable type (T).
            // This is necessary because Expression.Convert cannot directly convert a non-nullable value to a nullable type.
            // By first converting to the underlying type, we ensure compatibility before handling the nullable conversion.
            var nonNullableType = Nullable.GetUnderlyingType(targetProperty.PropertyType);

            // Nullable.GetUnderlyingType(typeof(int?))  // returns typeof(int)
            // Nullable.GetUnderlyingType(typeof(int))   // returns null
            if (nonNullableType is null)
            {
                // Ensure proper conversion from object to the target type
                var changeTypeCall = Expression.Call(
                    typeof(Convert),
                    nameof(Convert.ChangeType),
                    Type.EmptyTypes,
                    sourceValue,
                    Expression.Constant(targetProperty.PropertyType));

                var convertedValue = Expression.ConvertChecked(changeTypeCall, targetProperty.PropertyType);

                yield return Expression.Bind(targetProperty, convertedValue);
            }
            else
            {
                var isNullCheck = Expression.Equal(sourceValue, Expression.Constant(null));

                var defaultValue = Expression.Convert(
                    Expression.Call(typeof(Activator), nameof(Activator.CreateInstance), Type.EmptyTypes, Expression.Constant(nonNullableType)),
                    nonNullableType);

                var changeTypeCall = Expression.Call(
                    typeof(Convert),
                    nameof(Convert.ChangeType),
                    Type.EmptyTypes,
                    sourceValue,
                    Expression.Constant(nonNullableType));

                // Use Convert.ChangeType to dynamically convert the value
                var conversion = Expression.ConvertChecked(
                    changeTypeCall,
                    nonNullableType);

                // Use a conditional expression: if (value == null) default(T) else Convert.ChangeType(...)
                var fallbackDefaultValue = Expression.Condition(
                    isNullCheck,
                    defaultValue,  // Use default value if null
                    conversion);     // Otherwise, use the converted value

                var convertedValue = Expression.Convert(fallbackDefaultValue, targetProperty.PropertyType);

                yield return Expression.Bind(targetProperty, convertedValue);
            }
        }
    }

    private IEnumerable<MemberInitExpression> GroupCreateMemberInit()
    {
        foreach (var (retrieve, (alias, props)) in MapProfiles)
        {
            yield return Expression.MemberInit(Expression.New(retrieve), GroupCreateBindings(alias, props));
        }
    }

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    public List<TReturn> GetGroupMap()
    {
        // Define the target types for the ValueTuple, combining the row type and mapping profile keys.
        Type[] groupingTargetTypes = [.. GroupingPropertyAssignmentProcessing.Select(i => i.GroupingKeyType)];
        Expression[] groupingPropertyAssignments = [.. GroupingPropertyAssignmentProcessing.Select(i => i.PropertyAssignment)];

        // Dynamically create a ValueTuple type that will store grouped values.
        Type groupingKeyType = Type.GetType($"{typeof(ValueTuple).FullName}`{groupingTargetTypes.Length}")!.MakeGenericType(groupingTargetTypes);

        // Retrieve the constructor for the dynamically generated ValueTuple.
        ConstructorInfo groupingKeyConstructor = groupingKeyType.GetConstructor(groupingTargetTypes)!;

        // Construct a new ValueTuple instance for each row, initializing it with row data and group mappings.
        UnaryExpression groupingKeyVariable = Expression.Convert(Expression.New(groupingKeyConstructor, groupingPropertyAssignments), typeof(ITuple));
        IndexExpression groupingKeyIndex = Expression.Property(GroupingDapperRowParameter, "Item", groupingKeyVariable);

        //
        UnaryExpression rowNum = ChangeType(Expression.Property(DapperRowVariable, "Item", Expression.Constant("RowNum")), typeof(int));

        //
        ConditionalExpression firstRowProcessing = Expression.IfThen(
            Expression.Equal(rowNum, Expression.Constant(1)),
            Expression.Assign(groupingKeyIndex, Expression.New(typeof(Dictionary<string, TReturn>))));

        //
        MemberInitExpression retrieveInit = Expression.MemberInit(Expression.New(typeof(TReturn)), RetrievePropertyAssignmentProcessing);
        Expression retrieveId = ChangeType(Expression.Property(DapperRowVariable, "Item", Expression.Constant("Extend0_Id")), typeof(string));

        //
        // if (!groupingKeyIndex.TryGetValue(retrieveId, out RetrieveVariable))
        // if (!groupingKeyIndex.TryGetValue(retrieveId, out var value))
        // {
        //     // RetrieveVariable = retrieveInit;
        //     value = retrieveInit;
        //     // groupingKeyIndex[retrieveId] = RetrieveVariable;
        //     groupingKeyIndex[retrieveId] = value;
        // }
        ConditionalExpression tryGetProcessing = Expression.IfThen(
            Expression.Not(
                Expression.Call(
                    groupingKeyIndex,
                    typeof(Dictionary<string, TReturn>).GetMethod("TryGetValue")!,
                    retrieveId,
                    RetrieveVariable)),
            Expression.Block(
                Expression.Assign(RetrieveVariable, retrieveInit),
                Expression.Call(
                    groupingKeyIndex,
                    typeof(Dictionary<string, TReturn>).GetMethod("set_Item")!,
                    retrieveId,
                    RetrieveVariable)));

        // Build while-loop
        MemberExpression currentDapperRow = Expression.Property(DapperRowStreamVariable, "Current");
        BlockExpression whileBody = Expression.Block(
            [DapperRowVariable, RetrieveVariable],
            [Expression.Assign(DapperRowVariable, currentDapperRow), firstRowProcessing, tryGetProcessing, .. SequenceDapperRowProcessing]);

        LabelTarget breakLabel = Expression.Label();
        LoopExpression whileLoop = Expression.Loop(
            Expression.IfThenElse(
                Expression.Call(DapperRowStreamVariable, StreamMoveNextMethod),
                whileBody,
                Expression.Break(breakLabel)),
            breakLabel);

        BlockExpression fullBlock = Expression.Block(
            [DapperRowStreamVariable],
            [Expression.Assign(DapperRowStreamVariable, Expression.Call(DapperRowCollectionParameter, DapperRowStreamMethod)), whileLoop]);

        var lambda = Expression.Lambda<Action<IEnumerable<IDictionary<string, object>>, Dictionary<ITuple, Dictionary<string, TReturn>>>>(fullBlock, [DapperRowCollectionParameter, GroupingDapperRowParameter]).Compile();

        Dictionary<ITuple, Dictionary<string, TReturn>> res = [];

        var dtRows = Connection.Query(Sql, Parameters)
                .Cast<IDictionary<string, object>>()
                .ToList();

        lambda(dtRows, res);

        // Return an empty list as placeholder (if you intend to return results, modify this).
        return [];
    }

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    public List<TReturn> GetSingleMap()
        => [.. Connection.Query<TReturn>(Sql, Parameters)];

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    public List<TReturn> GetMultiMap()
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
            RetrieveVariable);

        var initializer = Expression.MemberInit(Expression.New(RetrieveEntity), CreateBindings());

        // Define a block to add a new record to the dictionary if TryGetValue fails
        var addNewToDictBlock = Expression.Block(
            Expression.Assign(RetrieveVariable, initializer),
            Expression.Call(
                dictConst,
                dictType.GetMethod("Add")!,
                Expression.Property(SourceParameter, "Id"),
                RetrieveVariable));

        // If the record is not in the dictionary (TryGetValue returns false), add it using the block defined above
        var ifNotInDict = Expression.IfThen(Expression.IsFalse(tryGetCall), addNewToDictBlock);

        // Define parameters for the lambda expression
        var parameters = BlockMapSequence.Select(e => e.Parameter).ToList();
        var entities = parameters.Select(e => e.Type).ToList();

        // Define multiple expressions (Access method).
        var methods = BlockMapSequence.Select(e => e.Expr).ToList();

        // Combine expressions into a block: check dictionary, execute additional methods, and return result
        var block = Expression.Block([RetrieveVariable], [ifNotInDict, .. methods, RetrieveVariable]);

        // Create a list of parameter types for the query method
        List<Type> types = [SourceEntity, .. entities, RetrieveEntity];

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
