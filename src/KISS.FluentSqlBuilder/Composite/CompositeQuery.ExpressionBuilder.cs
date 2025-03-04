namespace KISS.FluentSqlBuilder.Composite;

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

                yield return Expression.Bind(targetProperty, ChangeType(sourceValue, targetProperty.PropertyType));
            }
        }
    }

    /// <summary>
    ///     Converts an <see cref="IndexExpression"/> value, typically sourced from a dictionary-like structure,
    ///     to a specified target type, handling both nullable and non-nullable conversions.
    /// </summary>
    /// <param name="sourceValue">
    ///     The source value as an <see cref="IndexExpression"/>, assumed to originate
    ///     from an <see cref="IDictionary{TKey, TValue}"/> such as <see cref="IDictionary{string, object}"/>.
    ///     This value may represent a string, null, or other type requiring conversion.
    /// </param>
    /// <param name="targetType">
    ///     The desired target type for conversion, which may be nullable
    ///     (e.g., <see cref="Nullable{T}"/>) or non-nullable (e.g., <see cref="Guid"/> or <see cref="int"/>).
    /// </param>
    /// <returns>
    ///     An <see cref="Expression"/> representing the converted value, adjusted to match the
    ///     <paramref name="targetType"/>. For nullable types, includes null checks to handle null source values
    ///     appropriately.
    /// </returns>
    private Expression ChangeType(IndexExpression sourceValue, Type targetType)
    {
        // If the target type is nullable (e.g., Nullable<T>), retrieve its underlying non-nullable type (T).
        // This is necessary because Expression.Convert cannot directly convert a non-nullable value to a nullable type.
        // By first converting to the underlying type, we ensure compatibility before handling the nullable conversion.
        var nonNullableType = Nullable.GetUnderlyingType(targetType);
        var effectiveTargetType = nonNullableType ?? targetType;

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
                    Expression.Convert(defaultValue, targetType),
                    Expression.Convert(parseGuidCall, targetType));
            }
            else // Guid
            {
                convertedValue = parseGuidCall;
            }

            return convertedValue;
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
                    Expression.Constant(targetType));

                var convertedValue = Expression.ConvertChecked(changeTypeCall, targetType);

                return convertedValue;
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

                var convertedValue = Expression.Convert(fallbackDefaultValue, targetType);

                return convertedValue;
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
    /// <param name="inputData">
    ///     The list of dictionaries representing the raw data to process, where each dictionary contains key-value pairs
    ///     for a single row (e.g., "Extend0_Id" mapped to a value).
    /// </param>
    /// <returns>The populated collection of type <typeparamref name="TReturn" /> containing the processed entities.</returns>
    /// <remarks>
    ///     This method leverages expression trees to dynamically construct a processing pipeline,
    ///     ensuring flexibility and performance.
    ///     The <see cref="IterRowProcessors" /> collection must be defined elsewhere (e.g., as a field or property) and
    ///     provide the logic for transforming each row into an entity and adding it to the output collection.
    /// </remarks>
    private List<TEntity> SimpleProcess<TEntity>(List<IDictionary<string, object>> inputData)
    {
        var returnType = typeof(List<TEntity>);

        // Enumerator for iterating over inputData
        ParameterExpression itorVariable = Expression.Variable(typeof(IEnumerator<IDictionary<string, object>>), "itor"),
            // Current row being processed in the loop
            iterationRowParameter = Expression.Parameter(typeof(IDictionary<string, object>), "iter"),
            // Entity being constructed/modified for the current row
            currentEntityVariable = Expression.Variable(typeof(TEntity), "currentEntity"),
            // Collection that accumulates all processed entities
            outputCollectionVariable = Expression.Variable(returnType, "output");

        // Label to exit the loop when no more rows remain
        var breakLabel = Expression.Label();
        var whileLoopBody = Expression.Loop(
            Expression.IfThenElse(
                Expression.Call(itorVariable, ItorMoveNextMethod), // If MoveNext returns true (more rows),
                Expression.Block(
                    [iterationRowParameter, currentEntityVariable],
                    [
                        // Execute the loop body with the current row
                        Expression.Assign(iterationRowParameter, Expression.Property(itorVariable, "Current")),
                        Expression.Block(
                            [currentEntityVariable], // Defines a block with the current entity variable.
                            IterRowProcessor((iterationRowParameter, currentEntityVariable)),
                            Expression.Call(
                                outputCollectionVariable, // Calls the Add method on the output list.
                                returnType.GetMethod("Add")!, // Retrieves the Add method via reflection.
                                currentEntityVariable))
                    ]),
                Expression.Break(breakLabel)), // Otherwise, break out of the loop
            breakLabel); // Target label for breaking the loop

        var whileBlock = Expression.TryFinally(
            whileLoopBody, // The loop processing all rows
            Expression.Call(itorVariable, DisposeMethod)); // Ensures the enumerator is disposed after completion

        var fullBlock = Expression.Block(
            [outputCollectionVariable, itorVariable], // Declares variables used in the block
            [
                // Initializes outputCollection with a new instance of T
                Expression.Assign(outputCollectionVariable, Expression.New(returnType)),
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
        return Expression.Lambda<Func<List<TEntity>>>(fullBlock).Compile()();
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
    /// <param name="inputData">
    ///     The list of dictionaries representing the raw data to process, where each dictionary contains key-value pairs
    ///     for a single row (e.g., "Extend0_Id" mapped to a value).
    /// </param>
    /// <returns>The populated collection of type <typeparamref name="TReturn" /> containing the processed entities.</returns>
    /// <remarks>
    ///     This method leverages expression trees to dynamically construct a processing pipeline,
    ///     ensuring flexibility and performance.
    ///     The <see cref="IterRowProcessors" /> collection must be defined elsewhere (e.g., as a field or property) and
    ///     provide the logic for transforming each row into an entity and adding it to the output collection.
    /// </remarks>
    private List<TEntity> DictProcess<TEntity>(List<IDictionary<string, object>> inputData)
    {
        var returnType = typeof(Dictionary<string, TEntity>);

        // Enumerator for iterating over inputData
        ParameterExpression itorVariable = Expression.Variable(typeof(IEnumerator<IDictionary<string, object>>), "itor"),
            // Current row being processed in the loop
            iterationRowParameter = Expression.Parameter(typeof(IDictionary<string, object>), "iter"),
            // Entity being constructed/modified for the current row
            currentEntityVariable = Expression.Variable(typeof(TEntity), "currentEntity"),
            // Collection that accumulates all processed entities
            outputCollectionVariable = Expression.Variable(returnType, "output"),
            keyVariable = Expression.Variable(typeof(string), "key");

        var indexer = Expression.MakeIndex(
            outputCollectionVariable,
            typeof(Dictionary<string, TEntity>).GetProperty("Item"),
            [keyVariable]);

        // Label to exit the loop when no more rows remain
        var breakLabel = Expression.Label();
        var whileLoopBody = Expression.Loop(
            Expression.IfThenElse(
                Expression.Call(itorVariable, ItorMoveNextMethod), // If MoveNext returns true (more rows),
                Expression.Block(
                    [iterationRowParameter, currentEntityVariable, keyVariable],
                    [
                        Expression.Assign(iterationRowParameter, Expression.Property(itorVariable, "Current")),
                        Expression.Assign(
                            keyVariable,
                            ChangeType(Expression.Property(iterationRowParameter, "Item", Expression.Constant("Extend0_Id")), typeof(string))),
                        Expression.IfThen(
                            Expression.Not(Expression.Call(
                                outputCollectionVariable,
                                typeof(Dictionary<string, TEntity>).GetMethod("ContainsKey")!,
                                keyVariable)),
                            Expression.Block(
                                [currentEntityVariable],
                                IterRowProcessor((iterationRowParameter, currentEntityVariable)),
                                Expression.Call(
                                    outputCollectionVariable,
                                    typeof(Dictionary<string, TEntity>).GetMethod("Add")!,
                                    keyVariable,
                                    currentEntityVariable))),
                        .. JoinRowProcessors.Select(processor => processor((iterationRowParameter, indexer)))
                    ]),
                Expression.Break(breakLabel)), // Otherwise, break out of the loop
            breakLabel); // Target label for breaking the loop

        var whileBlock = Expression.TryFinally(
            whileLoopBody, // The loop processing all rows
            Expression.Call(itorVariable, DisposeMethod)); // Ensures the enumerator is disposed after completion

        var fullBlock = Expression.Block(
            [outputCollectionVariable, itorVariable], // Declares variables used in the block
            [
                // Initializes outputCollection with a new instance of T
                Expression.Assign(outputCollectionVariable, Expression.New(returnType)),
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
        var res = Expression.Lambda<Func<Dictionary<string, TEntity>>>(fullBlock).Compile()();
        return [.. res.Values];
    }
}
