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
    ///     The source value as an <see cref="IndexExpression"/>,
    ///     assumed to originate from an <see cref="IDictionary{TKey, TValue}"/>.
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
    ///     Processes a list of dictionary-based input data into a collection of type <typeparamref name="TEntity" />
    ///     using expression trees. This method iterates over the input data,
    ///     constructs entities of type <typeparamref name="TEntity" />,
    ///     and accumulates them into the output collection based on a set of processor expressions
    ///     defined in <see cref="IterRowProcessor" />.
    ///     The processing logic is built dynamically and executed via a compiled lambda expression for efficiency.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     The type of entity constructed or modified for each row of input data (e.g., a custom entity class).
    /// </typeparam>
    /// <param name="inputData">
    ///     The list of dictionaries representing the raw data to process, where each dictionary contains key-value pairs
    ///     for a single row (e.g., "Extend0_Id" mapped to a value).
    /// </param>
    /// <returns>The populated collection of type <typeparamref name="TEntity" /> containing the processed entities.</returns>
    /// <remarks>
    ///     This method leverages expression trees to dynamically construct a processing pipeline,
    ///     ensuring flexibility and performance.
    ///     The <see cref="IterRowProcessor" /> collection must be defined elsewhere (e.g., as a field or property) and
    ///     provide the logic for transforming each row into an entity and adding it to the output collection.
    /// </remarks>
    private List<TEntity> SimpleProcessToList<TEntity>(List<IDictionary<string, object>> inputData)
    {
        var returnType = typeof(List<TEntity>);

        // Enumerator for iterating over inputData
        ParameterExpression inputDictItorVariable =
                Expression.Variable(typeof(IEnumerator<IDictionary<string, object>>), "inputDictItorVariable"),
            // Current row being processed in the loop
            currentInputRowParameter = Expression.Parameter(typeof(IDictionary<string, object>), "currentInputRowParameter"),
            // Entity being constructed/modified for the current row
            currentEntityVariable = Expression.Variable(typeof(TEntity), "currentEntityVariable"),
            // Collection that accumulates all processed entities
            outputVariable = Expression.Variable(returnType, "outputVariable");

        // Label to exit the loop when no more rows remain
        var breakLabel = Expression.Label();
        var whileLoopBody = Expression.Loop(
            Expression.IfThenElse(
                Expression.Call(inputDictItorVariable, ItorMoveNextMethod), // If MoveNext returns true (more rows),
                Expression.Block(
                    [currentInputRowParameter, currentEntityVariable],
                    [
                        // Execute the loop body with the current row
                        Expression.Assign(currentInputRowParameter, Expression.Property(inputDictItorVariable, "Current")),
                        Expression.Block(
                            [currentEntityVariable], // Defines a block with the current entity variable.
                            IterRowProcessor((currentInputRowParameter, currentEntityVariable)),
                            Expression.Call(
                                outputVariable, // Calls the Add method on the output list.
                                returnType.GetMethod("Add")!, // Retrieves the Add method via reflection.
                                currentEntityVariable))
                    ]),
                Expression.Break(breakLabel)), // Otherwise, break out of the loop
            breakLabel); // Target label for breaking the loop

        var whileBlock = Expression.TryFinally(
            whileLoopBody, // The loop processing all rows
            Expression.Call(inputDictItorVariable, DisposeMethod)); // Ensures the enumerator is disposed after completion

        var fullBlock = Expression.Block(
            [outputVariable, inputDictItorVariable], // Declares variables used in the block
            [
                // Initializes outputCollection with a new instance of T
                Expression.Assign(outputVariable, Expression.New(returnType)),
                // Sets up the enumerator for inputData
                Expression.Assign(
                    inputDictItorVariable,
                    Expression.Call(Expression.Constant(inputData), GetEnumeratorForIEnumDictStrObj)),
                // Executes the loop with cleanup
                whileBlock,
                // Returns the populated collection
                outputVariable
            ]);

        // Compiles the expression tree
        var lambda = Expression.Lambda<Func<List<TEntity>>>(fullBlock).Compile();

        // Executes the expression tree, returning the result
        return lambda();
    }

    /// <summary>
    ///     Processes a list of dictionary-based input data into a collection of type <typeparamref name="TEntity" /> using
    ///     expression trees. This method iterates over the input data,
    ///     constructs entities of type <typeparamref name="TEntity" />,
    ///     and accumulates them into the output collection based on a set of processor expressions
    ///     defined in <see cref="IterRowProcessor" />.
    ///     The processing logic is built dynamically and executed via a compiled lambda expression for efficiency.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     The type of entity constructed or modified for each row of input data (e.g., a custom entity class).
    /// </typeparam>
    /// <param name="inputData">
    ///     The list of dictionaries representing the raw data to process, where each dictionary contains key-value pairs
    ///     for a single row (e.g., "Extend0_Id" mapped to a value).
    /// </param>
    /// <returns>The populated collection of type <typeparamref name="TEntity" /> containing the processed entities.</returns>
    /// <remarks>
    ///     This method leverages expression trees to dynamically construct a processing pipeline,
    ///     ensuring flexibility and performance.
    ///     The <see cref="IterRowProcessor" /> collection must be defined elsewhere (e.g., as a field or property) and
    ///     provide the logic for transforming each row into an entity and adding it to the output collection.
    /// </remarks>
    private List<TEntity> UniqueProcessToList<TEntity>(List<IDictionary<string, object>> inputData)
    {
        // Defines the type of the final output collection
        Type returnType = typeof(List<TEntity>),
            // Defines the type of the intermediate dictionary used for uniqueness.
            dictObjEntityType = typeof(Dictionary<object, TEntity>),
            // Specifies the type of the key used in the dictionary, extracted from each row for indexing.
            dictKeyType = typeof(object);

        // Enumerator for iterating over inputData
        ParameterExpression inputDictItorVariable =
                Expression.Variable(typeof(IEnumerator<IDictionary<string, object>>), "inputDictItorVariable"),
            // Current row being processed in the loop
            currentInputRowParameter = Expression.Parameter(typeof(IDictionary<string, object>), "currentInputRowParameter"),
            // Entity being constructed/modified for the current row
            currentEntityVariable = Expression.Variable(typeof(TEntity), "currentEntityVariable"),
            // Collection that accumulates all processed entities
            outputVariable = Expression.Variable(returnType, "outputVariable"),
            // Intermediate dictionary that ensures uniqueness of entities by key
            dictObjEntityVariable = Expression.Variable(dictObjEntityType, "dictObjEntityVariable"),
            // Key extracted from each row for dictionary indexing
            keyVariable = Expression.Variable(dictKeyType, "keyVariable");

        // Creates an indexer for accessing or setting dictionary entries (dictObjEntity[keyVariable]).
        var indexer = Expression.MakeIndex(
            dictObjEntityVariable,
            dictObjEntityType.GetProperty("Item")!,
            [keyVariable]);

        // Label to exit the loop when no more rows remain
        var breakLabel = Expression.Label();

        // Constructs the loop body that processes each row until the enumerator is exhausted.
        var whileLoopBody = Expression.Loop(
            Expression.IfThenElse(
                Expression.Call(inputDictItorVariable, ItorMoveNextMethod), // Checks if more rows exist by calling MoveNext
                Expression.Block(
                    [currentInputRowParameter, currentEntityVariable, keyVariable], // Local variables for this iteration
                    [
                        // Assigns the current row from the enumerator to iterationRowParameter.
                        Expression.Assign(currentInputRowParameter, Expression.Property(inputDictItorVariable, "Current")),
                        // Extracts and converts the "Extend0_Id" key from the row to a string.
                        Expression.Assign(
                            keyVariable,
                            ChangeType(
                                Expression.Property(currentInputRowParameter, "Item", Expression.Constant("Extend0_Id")),
                                dictKeyType)),
                        // Processes the row if its key isn’t already in the dictionary.
                        Expression.IfThen(
                            Expression.Not(Expression.Call(
                                dictObjEntityVariable,
                                dictObjEntityType.GetMethod("ContainsKey")!,
                                keyVariable)),
                            Expression.Block(
                                [], // Ensures variables is scoped for this operation
                                // Applies the row processor to construct or modify the entity.
                                IterRowProcessor((currentInputRowParameter, currentEntityVariable)),
                                // Adds the processed entity to the dictionary with its key.
                                Expression.Call(
                                    dictObjEntityVariable,
                                    dictObjEntityType.GetMethod("Add")!,
                                    keyVariable,
                                    currentEntityVariable))),
                        // Applies additional join processors using the dictionary indexer for related data.
                        .. JoinRowProcessors.Select(processor => processor((currentInputRowParameter, indexer)))
                    ]),
                Expression.Break(breakLabel)), // Exits the loop when no more rows remain
            breakLabel); // Target label for breaking the loop

        var whileBlock = Expression.TryFinally(
            whileLoopBody, // The loop processing all rows
            Expression.Call(inputDictItorVariable, DisposeMethod)); // Ensures the enumerator is disposed after completion

        var fullBlock = Expression.Block(
            [dictObjEntityVariable, inputDictItorVariable, outputVariable], // Declares variables used in the block
            [
                // Initializes dictObjEntity with a new instance of T
                Expression.Assign(dictObjEntityVariable, Expression.New(dictObjEntityType)),
                // Sets up the enumerator for inputData
                Expression.Assign(
                    inputDictItorVariable,
                    Expression.Call(Expression.Constant(inputData), GetEnumeratorForIEnumDictStrObj)),
                // Executes the loop with cleanup
                whileBlock,
                // Initialize outputList with a new list
                Expression.Assign(outputVariable, Expression.New(returnType)),
                // Convert dictionary values to list
                Expression.Call(
                    outputVariable,
                    returnType.GetMethod("AddRange")!,
                    Expression.Property(dictObjEntityVariable, "Values")),
                // Return the populated list
                outputVariable
            ]);

        // Compiles the expression tree
        var lambda = Expression.Lambda<Func<List<TEntity>>>(fullBlock).Compile();

        // Executes the expression tree, returning the result
        return lambda();
    }

    /// <summary>
    ///     Processes a list of dictionary-based input data into a collection of type <typeparamref name="TEntity" /> using
    ///     expression trees. This method iterates over the input data,
    ///     constructs entities of type <typeparamref name="TEntity" />,
    ///     and accumulates them into the output collection based on a set of processor expressions
    ///     defined in <see cref="IterRowProcessor" />.
    ///     The processing logic is built dynamically and executed via a compiled lambda expression for efficiency.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     The type of entity constructed or modified for each row of input data (e.g., a custom entity class).
    /// </typeparam>
    /// <param name="inputData">
    ///     The list of dictionaries representing the raw data to process, where each dictionary contains key-value pairs
    ///     for a single row (e.g., "Extend0_Id" mapped to a value).
    /// </param>
    /// <returns>The populated collection of type <typeparamref name="TEntity" /> containing the processed entities.</returns>
    /// <remarks>
    ///     This method leverages expression trees to dynamically construct a processing pipeline,
    ///     ensuring flexibility and performance.
    ///     The <see cref="IterRowProcessor" /> collection must be defined elsewhere (e.g., as a field or property) and
    ///     provide the logic for transforming each row into an entity and adding it to the output collection.
    /// </remarks>
    private Dictionary<ITuple, List<TEntity>> NestedUniqueProcessToList<TEntity>(List<IDictionary<string, object>> inputData)
    {
        // Defines the type of the final output collection
        Type returnType = typeof(Dictionary<ITuple, List<TEntity>>),
            // Defines the type of outer dictionary for the intermediate dictionary used for uniqueness.
            outerDictObjEntityType = typeof(Dictionary<ITuple, Dictionary<object, TEntity>>),
            // Specifies the type of the key used in the dictionary, extracted from each row for indexing.
            outerKeyType = typeof(ITuple),
            // Defines the type of the intermediate dictionary used for uniqueness.
            dictObjEntityType = typeof(Dictionary<object, TEntity>),
            // Specifies the type of the key used in the dictionary, extracted from each row for indexing.
            dictKeyType = typeof(object);

        // Enumerator for iterating over inputData
        ParameterExpression inputDictItorVariable =
                Expression.Variable(typeof(IEnumerator<IDictionary<string, object>>), "inputDictItorVariable"),
            // Current row being processed in the loop
            currentInputRowParameter = Expression.Parameter(typeof(IDictionary<string, object>), "currentInputRowParameter"),
            // Entity being constructed/modified for the current row
            currentEntityVariable = Expression.Variable(typeof(TEntity), "currentEntityVariable"),
            // Collection that accumulates all processed entities
            outputVariable = Expression.Variable(returnType, "outputVariable"),
            // Intermediate dictionary that ensures uniqueness of entities by key
            outerDictObjEntityVariable = Expression.Variable(outerDictObjEntityType, "outerDictObjEntityVariable"),
            // Key extracted from each row for dictionary indexing
            outerKeyVariable = Expression.Variable(outerKeyType, "outerKeyVariable"),
            keyVariable = Expression.Variable(dictKeyType, "keyVariable"),
            // Enumerator for iterating over the outer dictionary of entities
            outerDictItorVariable = Expression.Variable(typeof(Dictionary<ITuple, Dictionary<object, TEntity>>.Enumerator), "outerDictItorVariable"),
            // Current key-value pair from the outer dictionary being processed
            outerDictEntryParameter = Expression.Parameter(typeof(KeyValuePair<ITuple, Dictionary<object, TEntity>>), "outerDictEntryParameter");

        // Generate ValueTuple type dynamically
        var outerKeyValueTupleType = Type.GetType($"{typeof(ValueTuple).FullName}`{GroupingKeys.Count}")!.MakeGenericType([.. GroupingKeys.Values]);

        // Create constructor arguments from currentInputRowParameter
        var outerKeyConstructorArguments = GroupingKeys
            .Select(grp => ChangeType(Expression.Property(currentInputRowParameter, "Item", Expression.Constant(grp.Key)), grp.Value))
            .ToArray();

        // Gets Dictionary<string, TEntity>
        var outerIndexer = Expression.MakeIndex(
            outerDictObjEntityVariable,
            outerDictObjEntityType.GetProperty("Item")!,
            [outerKeyVariable]);

        // Creates an indexer for accessing or setting dictionary entries (outerDictObjEntity[outerKeyVariable][keyVariable]).
        var indexer = Expression.MakeIndex(
            outerIndexer,
            dictObjEntityType.GetProperty("Item")!,
            [keyVariable]);

        // Label to exit the loop when no more rows remain
        var breakLabel = Expression.Label();

        // Constructs the loop body that processes each row until the enumerator is exhausted.
        var whileLoopBody = Expression.Loop(
            Expression.IfThenElse(
                Expression.Call(inputDictItorVariable, ItorMoveNextMethod), // Checks if more rows exist by calling MoveNext
                Expression.Block(
                    [currentInputRowParameter, currentEntityVariable, outerKeyVariable, keyVariable], // Local variables for this iteration
                    [
                        // Assigns the current row from the enumerator to iterationRowParameter.
                        Expression.Assign(currentInputRowParameter, Expression.Property(inputDictItorVariable, "Current")),
                        Expression.Assign(
                            outerKeyVariable,
                            Expression.Convert(
                                Expression.New(
                                    outerKeyValueTupleType.GetConstructor([.. GroupingKeys.Values])!,
                                    outerKeyConstructorArguments),
                                outerKeyType)),
                        // Processes the row if its key isn’t already in the dictionary.
                        Expression.IfThen(
                            Expression.Not(Expression.Call(
                                outerDictObjEntityVariable,
                                outerDictObjEntityType.GetMethod("ContainsKey")!,
                                outerKeyVariable)),
                            Expression.Block(
                                [],
                                // Adds the processed entity to the dictionary with its key.
                                Expression.Call(
                                    outerDictObjEntityVariable,
                                    outerDictObjEntityType.GetMethod("Add")!,
                                    outerKeyVariable,
                                    Expression.New(dictObjEntityType)))),
                        // Extracts and converts the "Extend0_Id" key from the row to a string.
                        Expression.Assign(
                            keyVariable,
                            ChangeType(
                                Expression.Property(currentInputRowParameter, "Item", Expression.Constant("Extend0_Id")),
                                dictKeyType)),
                        // Processes the row if its key isn’t already in the dictionary.
                        Expression.IfThen(
                            Expression.Not(Expression.Call(
                                outerIndexer,
                                dictObjEntityType.GetMethod("ContainsKey")!,
                                keyVariable)),
                            Expression.Block(
                                [], // Ensures variables is scoped for this operation
                                // Applies the row processor to construct or modify the entity.
                                IterRowProcessor((currentInputRowParameter, currentEntityVariable)),
                                // Adds the processed entity to the dictionary with its key.
                                Expression.Call(
                                    outerIndexer,
                                    dictObjEntityType.GetMethod("Add")!,
                                    keyVariable,
                                    currentEntityVariable))),
                        // Applies additional join processors using the dictionary indexer for related data.
                        .. JoinRowProcessors.Select(processor => processor((currentInputRowParameter, indexer)))
                    ]),
                Expression.Break(breakLabel)), // Exits the loop when no more rows remain
            breakLabel); // Target label for breaking the loop

        var whileBlock = Expression.TryFinally(
            whileLoopBody, // The loop processing all rows
            Expression.Call(inputDictItorVariable, DisposeMethod)); // Ensures the enumerator is disposed after completion

        var fullBlock = Expression.Block(
            [outerDictObjEntityVariable, inputDictItorVariable, outerDictItorVariable, outputVariable], // Declares variables used in the block
            [
                // Initializes dictObjEntity with a new instance of T
                Expression.Assign(outerDictObjEntityVariable, Expression.New(outerDictObjEntityType)),
                // Sets up the enumerator for inputData
                Expression.Assign(
                    inputDictItorVariable,
                    Expression.Call(Expression.Constant(inputData), GetEnumeratorForIEnumDictStrObj)),
                // Executes the loop with cleanup
                whileBlock,
                // Initialize outputList with a new list
                Expression.Assign(outputVariable, Expression.New(returnType)),
                // Set up the enumerator for outerDictObjEntityVariable
                Expression.Assign(
                    outerDictItorVariable,
                    Expression.Call(outerDictObjEntityVariable, outerDictObjEntityType.GetMethod("GetEnumerator")!)),
                // Loop over the dictionary entries
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Call(outerDictItorVariable, ItorMoveNextMethod),
                        Expression.Block(
                            [outerDictEntryParameter],
                            // Assigns the current row from the enumerator to iterationRowParameter.
                            Expression.Assign(outerDictEntryParameter, Expression.Property(outerDictItorVariable, "Current")),
                            Expression.Call(
                                outputVariable,
                                returnType.GetMethod("Add")!,
                                Expression.Property(outerDictEntryParameter, "Key"),
                                Expression.New(
                                    typeof(List<TEntity>).GetConstructor([typeof(IEnumerable<TEntity>)])!,
                                    Expression.Property(Expression.Property(outerDictEntryParameter, "Value"), "Values")))), // Add each key and its inner Values as a List<TEntity>
                        Expression.Break(breakLabel)),
                    breakLabel),
                // Dispose the enumerator
                Expression.Call(outerDictItorVariable, DisposeMethod),
                // Return the populated list
                outputVariable
            ]);

        // Compiles the expression tree
        var lambda = Expression.Lambda<Func<Dictionary<ITuple, List<TEntity>>>>(fullBlock).Compile();

        // Executes the expression tree, returning the result
        return lambda();
    }
}
