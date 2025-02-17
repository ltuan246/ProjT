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
public sealed partial class CompositeQuery(DbConnection connection, Type sourceEntity, Type retrieveEntity) : IDataRetrieval
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
    public List<TReturn> GetGroupMap<TReturn>()
    {
        // Define the target types for the ValueTuple, combining the row type and mapping profile keys.
        Type[] targetTypes = [DtRowType, .. MapProfiles.Keys];

        // Dynamically create a ValueTuple type that will store grouped values.
        Type tupleType = Type.GetType($"{typeof(ValueTuple).FullName}`{targetTypes.Length}")!.MakeGenericType(targetTypes);

        // Retrieve the constructor for the dynamically generated ValueTuple.
        ConstructorInfo tupleConstructor = tupleType.GetConstructor(targetTypes)!;

        // Construct a new ValueTuple instance for each row, initializing it with row data and group mappings.
        var tupleCreation = Expression.New(tupleConstructor, [DtRowParam, .. GroupCreateMemberInit()]);

        // Define a lambda function: (row) => new ValueTuple<>(row, group values).
        var transformLambda = Expression.Lambda(
            typeof(Func<,>).MakeGenericType([DtRowType, tupleType]),
            tupleCreation,
            DtRowParam);

        // Create a parameter expression representing a record in the tuple.
        var tupleParam = Expression.Parameter(tupleType, "rec");

        // Extract the fields from the dynamically created tuple.
        var fields = tupleType.GetFields();

        // Access the dictionary-like data from the first field in the tuple.
        var dictAccess = Expression.Field(tupleParam, fields[0]);

        // Generate expressions for extracting dictionary keys from grouped data.
        var exDictKeys = GroupKeys
            .Select(k => Expression.Property(dictAccess, "Item", Expression.Constant(k.GroupKey)))
            .ToList();

        // Dynamically create a ValueTuple type to store dictionary keys.
        Type[] dictKeyTypes = [.. Enumerable.Repeat(typeof(object), exDictKeys.Count)];
        var dictKeyValueTuple = Type.GetType($"{typeof(ValueTuple).FullName}`{exDictKeys.Count}")!.MakeGenericType(dictKeyTypes);

        // Retrieve the constructor for the dictionary key tuple.
        ConstructorInfo dictKeyConstructor = dictKeyValueTuple.GetConstructor(dictKeyTypes)!;

        // Construct the dictionary key tuple dynamically.
        var dictKeyCreation = Expression.New(dictKeyConstructor, exDictKeys);

        // Extract the primary object from the second field in the tuple.
        var firstObjectAccess = Expression.Field(tupleParam, fields[1]);

        // Dynamically create a dictionary type: Dictionary<(key tuple), List<object>>.
        Type dictType = typeof(Dictionary<,>).MakeGenericType(dictKeyValueTuple, typeof(List<object>));

        // Instantiate the dictionary at runtime.
        var returnDict = Activator.CreateInstance(dictType)!;
        var resDict = Expression.Constant(returnDict);

        // Retrieve dictionary methods for operations.
        var containsKeyMethod = dictType.GetMethod("ContainsKey")!;
        var addToDictMethod = dictType.GetMethod("Add")!;
        var addToListMethod = typeof(List<object>).GetMethod("Add")!;

        // If the dictionary does not contain the generated key, initialize a new list.
        var createList = Expression.IfThen(
            Expression.Not(Expression.Call(resDict, containsKeyMethod, dictKeyCreation)),
            Expression.Call(resDict, addToDictMethod, dictKeyCreation, Expression.New(typeof(List<object>))));

        // Access the list corresponding to the generated key and add the primary object.
        var itemDict = Expression.Property(resDict, "Item", dictKeyCreation);
        var addToList = Expression.Call(itemDict, addToListMethod, firstObjectAccess);

        // Combine dictionary initialization and object addition into a single expression block.
        var bodyBlock = Expression.Block(createList, addToList, firstObjectAccess);

        // Define the lambda for processing each tuple: (tuple) => { add to dictionary }.
        var selectLambda = Expression.Lambda(
            typeof(Func<,>).MakeGenericType(tupleType, typeof(object)),
            bodyBlock,
            tupleParam);

        // Define a parameter for the list of dictionaries (result set).
        var listParam = Expression.Parameter(typeof(List<IDictionary<string, object>>), "list");

        // Apply transformation: Select(list, transformLambda).
        var selectTransform = Expression.Call(
            typeof(Enumerable),
            "Select",
            [DtRowType, tupleType],
            listParam,
            transformLambda);

        // Apply second transformation: Select(selectTransform, selectLambda).
        var selectFinal = Expression.Call(
            typeof(Enumerable),
            "Select",
            [tupleType, typeof(object)],
            selectTransform,
            selectLambda);

        // Convert the result to a list.
        var toListCall = Expression.Call(
            typeof(Enumerable),
            "ToList",
            [typeof(object)],
            selectFinal);

        // Compile the full transformation pipeline into an executable delegate.
        var queryPipeline = Expression.Lambda<Func<List<IDictionary<string, object>>, List<object>>>(toListCall, listParam).Compile();

        // Retrieve data from the database, casting each row to IDictionary<string, object>.
        var dtRows = Connection.Query(Sql, Parameters)
            .Cast<IDictionary<string, object>>()
            .ToList();

        // Execute the transformation pipeline on the retrieved rows.
        _ = queryPipeline(dtRows);

        // Return an empty list as placeholder (if you intend to return results, modify this).
        return [];
    }

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    public List<TReturn> GetSingleMap<TReturn>()
        => [.. Connection.Query<TReturn>(Sql, Parameters)];

    /// <summary>
    ///     Executes the SQL query and returns the results as a list.
    /// </summary>
    /// <typeparam name="TReturn">The combined type to return.</typeparam>
    /// <returns>Retrieve the data based on conditions.</returns>
    public List<TReturn> GetMultiMap<TReturn>()
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
        var block = Expression.Block([RetrieveParameter], [ifNotInDict, .. methods, RetrieveParameter]);

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
