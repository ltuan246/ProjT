namespace KISS.FluentSqlBuilder.QueryHandlerChain;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
public sealed partial class CompositeQuery
{
    /// <summary>
    ///     Creates a collection of <see cref="MemberBinding" /> instances by mapping properties
    ///     from an iterated row (e.g., a dictionary or dynamic object) to a target type.
    /// </summary>
    /// <param name="itorRowVariable">The current row being processed in the loop.</param>
    /// <param name="sourceType">The type of the source entity providing the data.</param>
    /// <param name="targetType">The type of the target entity to which properties are bound.</param>
    /// <returns>
    ///     An enumerable collection of <see cref="MemberBinding" /> objects,
    ///     where each binding represents the assignment of a source property value
    ///     to a corresponding target property.
    /// </returns>
    public IEnumerable<MemberBinding> CreateIterRowBindings(
        ParameterExpression itorRowVariable,
        Type sourceType,
        Type targetType)
    {
        var alias = GetAliasMapping(sourceType);
        var sourceProperties = sourceType.GetProperties().Where(p => p.CanWrite).ToList();
        foreach (var sourceProperty in sourceProperties)
        {
            var targetProperty = targetType.GetProperty(sourceProperty.Name);
            if (targetProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
            {
                var sourceValue = Expression.Property(
                    itorRowVariable,
                    "Item",
                    Expression.Constant($"{alias}_{sourceProperty.Name}"));

                // If the target type is nullable (e.g., Nullable<T>), retrieve its underlying non-nullable type (T).
                // This is necessary because Expression.Convert cannot directly convert a non-nullable value to a nullable type.
                // By first converting to the underlying type, we ensure compatibility before handling the nullable conversion.
                var nonNullableType = Nullable.GetUnderlyingType(targetProperty.PropertyType);
                var effectiveTargetType = nonNullableType ?? targetProperty.PropertyType;

                // Handle specific conversion from string to Guid
                if (effectiveTargetType == typeof(Guid))
                {
                    // Assume sourceValue is a string from IDictionary<string, object>
                    var parseGuidCall = Expression.Call(
                        typeof(Guid),
                        nameof(Guid.Parse),
                        Type.EmptyTypes,
                        Expression.Convert(sourceValue, typeof(string)));

                    Expression convertedValue;
                    if (nonNullableType != null) // Nullable<Guid>
                    {
                        var isNullCheck = Expression.Equal(sourceValue, Expression.Constant(null));
                        var defaultValue = Expression.Default(nonNullableType); // null for Nullable<Guid>
                        convertedValue = Expression.Condition(
                            isNullCheck,
                            Expression.Convert(defaultValue, targetProperty.PropertyType),
                            Expression.Convert(parseGuidCall, targetProperty.PropertyType));
                    }
                    else // Guid
                    {
                        convertedValue = parseGuidCall;
                    }

                    yield return Expression.Bind(targetProperty, convertedValue);
                }
                else
                {
                    // General conversion logic for other types
                    if (nonNullableType is null)
                    {
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
                            Expression.Call(
                                typeof(Activator),
                                nameof(Activator.CreateInstance),
                                Type.EmptyTypes,
                                Expression.Constant(nonNullableType)),
                            nonNullableType);

                        var changeTypeCall = Expression.Call(
                            typeof(Convert),
                            nameof(Convert.ChangeType),
                            Type.EmptyTypes,
                            sourceValue,
                            Expression.Constant(nonNullableType));

                        var conversion = Expression.ConvertChecked(changeTypeCall, nonNullableType);
                        var fallbackDefaultValue = Expression.Condition(
                            isNullCheck,
                            defaultValue,
                            conversion);

                        var convertedValue = Expression.Convert(fallbackDefaultValue, targetProperty.PropertyType);
                        yield return Expression.Bind(targetProperty, convertedValue);
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Processes a list of dictionary-based input data into a collection of type <typeparamref name="TReturn" /> using
    ///     expression trees. This method iterates over the input data, constructs entities of type
    ///     <typeparamref name="TEntity" />,
    ///     and accumulates them into the output collection based on a set of processor expressions defined in
    ///     <see cref="IterRowProcessors" />.
    ///     The processing logic is built dynamically and executed via a compiled lambda expression for efficiency.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     The type of entity constructed or modified for each row of input data (e.g., a custom entity class).
    /// </typeparam>
    /// <typeparam name="TReturn">
    ///     The type of the output collection that accumulates the processed entities (e.g., <see cref="List{TEntity}" /> or
    ///     <see cref="Dictionary{TKey, TEntity}" />).
    ///     Must have a parameterless constructor.
    /// </typeparam>
    /// <param name="inputData">
    ///     The list of dictionaries representing the raw data to process, where each dictionary contains key-value pairs
    ///     for a single row (e.g., "Extend0_Id" mapped to a value).
    /// </param>
    /// <param name="outputProcessor">
    ///     Gets or sets a function that defines how to add the CurrentEntityVariable to the OutputCollectionVariable
    ///     within an expression tree. This processor generates an expression that appends or integrates the current entity
    ///     into the output collection.
    /// </param>
    /// <returns>The populated collection of type <typeparamref name="TReturn" /> containing the processed entities.</returns>
    /// <remarks>
    ///     This method leverages expression trees to dynamically construct a processing pipeline,
    ///     ensuring flexibility and performance.
    ///     The <see cref="IterRowProcessors" /> collection must be defined elsewhere (e.g., as a field or property) and
    ///     provide the logic for transforming each row into an entity and adding it to the output collection.
    /// </remarks>
    private TReturn ProcessData<TEntity, TReturn>(
        List<IDictionary<string, object>> inputData,
        Func<(ParameterExpression OutputCollectionVariable, ParameterExpression CurrentEntityVariable), Expression>
            outputProcessor)
    {
        // Enumerator for iterating over inputData
        var itorVariable = Expression.Variable(typeof(IEnumerator<IDictionary<string, object>>), "itor");

        // Current row being processed in the loop
        var iterationRowParameter = Expression.Parameter(typeof(IDictionary<string, object>), "iter");

        // Entity being constructed/modified for the current row
        var currentEntityVariable = Expression.Variable(typeof(TEntity), "currentEntity");

        // Collection that accumulates all processed entities
        var outputCollectionVariable = Expression.Variable(typeof(TReturn), "output");

        // Build the loop body, also assigns the current row from the enumerator to iterationRowParameter
        List<Expression> loopBodyExpressions =
            [Expression.Assign(iterationRowParameter, Expression.Property(itorVariable, "Current"))];

        foreach (var processor in IterRowProcessors)
        {
            // Adds each processor's expression to the loop body
            loopBodyExpressions.Add(processor((iterationRowParameter, currentEntityVariable)));
        }

        // Adds processor expression to the loop body
        loopBodyExpressions.Add(outputProcessor((outputCollectionVariable, currentEntityVariable)));

        // Label to exit the loop when no more rows remain
        var breakLabel = Expression.Label();

        var whileLoopBody = Expression.Loop(
            Expression.IfThenElse(
                Expression.Call(itorVariable, ItorMoveNextMethod), // If MoveNext returns true (more rows),
                Expression.Block(
                    [iterationRowParameter, currentEntityVariable],
                    loopBodyExpressions), // Execute the loop body with the current row
                Expression.Break(breakLabel)), // Otherwise, break out of the loop
            breakLabel); // Target label for breaking the loop

        var whileBlock = Expression.TryFinally(
            whileLoopBody, // The loop processing all rows
            Expression.Call(itorVariable, DisposeMethod)); // Ensures the enumerator is disposed after completion

        var fullBlock = Expression.Block(
            [outputCollectionVariable, itorVariable], // Declares variables used in the block
            [
                // Initializes outputCollection with a new instance of T
                Expression.Assign(outputCollectionVariable, Expression.New(typeof(TReturn))),
                // Sets up the enumerator for inputData
                Expression.Assign(
                    itorVariable,
                    Expression.Call(Expression.Constant(inputData), GetEnumeratorForIEnumDictStrObj)),
                // Executes the loop with cleanup
                whileBlock,
                // Returns the populated collection
                outputCollectionVariable
            ]);

        // Compiles and executes the expression tree, returning the result
        return Expression.Lambda<Func<TReturn>>(fullBlock).Compile()();
    }
}
