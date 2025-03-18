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
                            [], // Defines a block with the current entity variable.
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
        ParameterExpression dictInputEnumeratorVariable =
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
        var dictKeyAccessor = Expression.MakeIndex(
            dictObjEntityVariable,
            dictObjEntityType.GetProperty("Item")!,
            [keyVariable]);

        // Sets the current row from the input data enumerator.
        Expression assignCurrentInputRowFromInputEnumerator = Expression.Assign(
            currentInputRowParameter,
            Expression.Property(dictInputEnumeratorVariable, "Current")),

            // Sets the dictionary key from the row’s "Extend0_Id" value.
            assignKeyVariableFromPrimaryKey = Expression.Assign(
                keyVariable,
                ChangeType(
                    Expression.Property(currentInputRowParameter, "Item", Expression.Constant("Extend0_Id")),
                    dictKeyType)),

            // Processes and adds the entity to the dictionary if the key is new.
            initializeEntityIfKeyMissing = Expression.IfThen(
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
                        currentEntityVariable)));

        var applyJoinProcessorsToInnerKeyAccessor = JoinRowProcessors
            .Select(processor => processor((currentInputRowParameter, dictKeyAccessor)))
            .ToList();

        // Label to exit the loop when no more rows remain
        var breakLabel = Expression.Label();

        // Constructs the loop body that processes each row until the enumerator is exhausted.
        var whileLoopBody = Expression.Loop(
            Expression.IfThenElse(
                Expression.Call(dictInputEnumeratorVariable, ItorMoveNextMethod), // Checks if more rows exist by calling MoveNext
                Expression.Block(
                    [currentInputRowParameter, currentEntityVariable, keyVariable], // Local variables for this iteration
                    [
                        // Assigns the current row from the enumerator to iterationRowParameter.
                        assignCurrentInputRowFromInputEnumerator,
                        // Extracts and converts the "Extend0_Id" key from the row to a string.
                        assignKeyVariableFromPrimaryKey,
                        // Processes the row if its key isn’t already in the dictionary.
                        initializeEntityIfKeyMissing,
                        // Applies additional join processors using the dictionary indexer for related data.
                        .. applyJoinProcessorsToInnerKeyAccessor
                    ]),
                Expression.Break(breakLabel)), // Exits the loop when no more rows remain
            breakLabel); // Target label for breaking the loop

        var whileBlock = Expression.TryFinally(
            whileLoopBody, // The loop processing all rows
            Expression.Call(dictInputEnumeratorVariable, DisposeMethod)); // Ensures the enumerator is disposed after completion

        // Initializes the dictionary with a new instance.
        Expression initializeDictVariable = Expression.Assign(
            dictObjEntityVariable,
            Expression.New(dictObjEntityType)),

            // Sets up the enumerator for the input data list.
            setupInputDataEnumerator = Expression.Assign(
                dictInputEnumeratorVariable,
                Expression.Call(Expression.Constant(inputData), GetEnumeratorForIEnumDictStrObj)),

            // Initialize outputList with a new list
            initializeOutputVariable = Expression.Assign(outputVariable, Expression.New(returnType)),

            // Adds the current values as a list to the output.
            addValuesToOutput = Expression.Call(
                outputVariable,
                returnType.GetMethod("AddRange")!,
                Expression.Property(dictObjEntityVariable, "Values"));

        var fullBlock = Expression.Block(
            [dictObjEntityVariable, dictInputEnumeratorVariable, outputVariable], // Declares variables used in the block
            [
                // Initializes dictObjEntity with a new instance of T
                initializeDictVariable,
                // Sets up the enumerator for inputData
                setupInputDataEnumerator,
                // Executes the loop with cleanup
                whileBlock,
                // Initialize outputList with a new list
                initializeOutputVariable,
                // Convert dictionary values to list
                addValuesToOutput,
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
        ParameterExpression dictInputEnumeratorVariable =
                Expression.Variable(typeof(IEnumerator<IDictionary<string, object>>), "dictInputEnumeratorVariable"),
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
            innerKeyVariable = Expression.Variable(dictKeyType, "keyVariable"),
            // Enumerator for iterating over the outer dictionary of entities
            outerDictEnumeratorVariable = Expression.Variable(typeof(Dictionary<ITuple, Dictionary<object, TEntity>>.Enumerator), "outerDictItorVariable"),
            // Current key-value pair from the outer dictionary being processed
            outerDictEntryParameter = Expression.Parameter(typeof(KeyValuePair<ITuple, Dictionary<object, TEntity>>), "outerDictEntryParameter");

        // Accesses the inner dictionary in the outer dictionary via the outer key.
        IndexExpression outerKeyAccessor = Expression.MakeIndex(
                outerDictObjEntityVariable,
                outerDictObjEntityType.GetProperty("Item")!,
                [outerKeyVariable]),
            // Creates an indexer for accessing or setting dictionary entries (outerDictObjEntity[outerKeyVariable][keyVariable]).
            innerKeyAccessor = Expression.MakeIndex(
                outerKeyAccessor,
                dictObjEntityType.GetProperty("Item")!,
                [innerKeyVariable]);

        // Generate ValueTuple type dynamically
        var outerKeyConstructor = Type.GetType($"{typeof(ValueTuple).FullName}`{GroupingKeys.Count + AggregationKeys.Count}")!
            .MakeGenericType([.. GroupingKeys.Values, .. AggregationKeys.Values])
            .GetConstructor([.. GroupingKeys.Values, .. AggregationKeys.Values])!;

        // Create constructor arguments from currentInputRowParameter
        var outerKeyArguments = GroupingKeys
            .Union(AggregationKeys)
            .Select(grp => ChangeType(Expression.Property(currentInputRowParameter, "Item", Expression.Constant(grp.Key)), grp.Value))
            .ToArray();

        // Sets the current row from the input data enumerator.
        Expression assignCurrentInputRowFromInputEnumerator = Expression.Assign(
                currentInputRowParameter,
                Expression.Property(dictInputEnumeratorVariable, "Current")),

            // Creates and assigns the outer key from grouping data.
            assignOuterKeyVariableFromGroupingData = Expression.Assign(
                outerKeyVariable,
                Expression.Convert(Expression.New(outerKeyConstructor, outerKeyArguments), outerKeyType)),

            // Sets the inner dictionary key from the row’s "Extend0_Id" value.
            assignInnerKeyVariableFromPrimaryKey = Expression.Assign(
                innerKeyVariable,
                ChangeType(
                    Expression.Property(currentInputRowParameter, "Item", Expression.Constant("Extend0_Id")),
                    dictKeyType)),

            // Initializes a new inner dictionary if the outer key is missing.
            initializeInnerDictIfOuterKeyMissing = Expression.IfThen(
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

            // Processes and adds the entity to the inner dictionary if the key is new.
            initializeEntityIfInnerKeyMissing = Expression.IfThen(
                Expression.Not(Expression.Call(
                    outerKeyAccessor,
                    dictObjEntityType.GetMethod("ContainsKey")!,
                    innerKeyVariable)),
                Expression.Block(
                    [], // Ensures variables is scoped for this operation
                        // Applies the row processor to construct or modify the entity.
                    IterRowProcessor((currentInputRowParameter, currentEntityVariable)),
                    // Adds the processed entity to the dictionary with its key.
                    Expression.Call(
                        outerKeyAccessor,
                        dictObjEntityType.GetMethod("Add")!,
                        innerKeyVariable,
                        currentEntityVariable)));

        var applyJoinProcessorsToInnerKeyAccessor = JoinRowProcessors
            .Select(processor => processor((currentInputRowParameter, innerKeyAccessor)))
            .ToList();

        // Label to exit the loop when no more rows remain
        var breakLabel = Expression.Label();

        // Constructs the loop body that processes each row until the enumerator is exhausted.
        var whileLoopBody = Expression.Loop(
            Expression.IfThenElse(
                Expression.Call(dictInputEnumeratorVariable, ItorMoveNextMethod), // Checks if more rows exist by calling MoveNext
                Expression.Block(
                    [currentInputRowParameter, currentEntityVariable, outerKeyVariable, innerKeyVariable], // Local variables for this iteration
                    [
                        // Sets the current row from the input data enumerator.
                        assignCurrentInputRowFromInputEnumerator,
                        // Creates and assigns the outer key from grouping data.
                        assignOuterKeyVariableFromGroupingData,
                        // Sets the inner dictionary key from the row’s "Extend0_Id" value.
                        assignInnerKeyVariableFromPrimaryKey,
                        // Initializes a new inner dictionary if the outer key is missing.
                        initializeInnerDictIfOuterKeyMissing,
                        // Processes and adds the entity to the inner dictionary if the key is new.
                        initializeEntityIfInnerKeyMissing,
                        // Applies additional join processors using the dictionary indexer for related data.
                        .. applyJoinProcessorsToInnerKeyAccessor
                    ]),
                Expression.Break(breakLabel)), // Exits the loop when no more rows remain
            breakLabel); // Target label for breaking the loop

        var whileBlock = Expression.TryFinally(
            whileLoopBody, // The loop processing all rows
            Expression.Call(dictInputEnumeratorVariable, DisposeMethod)); // Ensures the enumerator is disposed after completion

        // Initializes the outer dictionary with a new instance.
        Expression initializeOuterDict = Expression.Assign(
                outerDictObjEntityVariable,
                Expression.New(outerDictObjEntityType)),

            // Sets up the enumerator for the input data list.
            setupInputDataEnumerator = Expression.Assign(
                dictInputEnumeratorVariable,
                Expression.Call(Expression.Constant(inputData), GetEnumeratorForIEnumDictStrObj)),

            // Initializes the output list with a new instance.
            initializeOutputList = Expression.Assign(
                outputVariable,
                Expression.New(returnType)),

            // Sets up the enumerator for the outer dictionary to flatten it.
            setupOuterDictEnumerator = Expression.Assign(
                outerDictEnumeratorVariable,
                Expression.Call(outerDictObjEntityVariable, outerDictObjEntityType.GetMethod("GetEnumerator")!)),

            // Assigns the current entry from the outer dictionary enumerator.
            assignCurrentOuterEntryFromOuterDictEnumerator = Expression.Assign(
                outerDictEntryParameter,
                Expression.Property(outerDictEnumeratorVariable, "Current")),

            // Adds the current key and its inner values as a list to the output.
            addOuterKeyAndValuesToOutput = Expression.Call(
                outputVariable,
                returnType.GetMethod("Add")!,
                Expression.Property(outerDictEntryParameter, "Key"),
                Expression.New(
                    typeof(List<TEntity>).GetConstructor([typeof(IEnumerable<TEntity>)])!,
                    Expression.Property(Expression.Property(outerDictEntryParameter, "Value"), "Values"))),

            // Flattens the outer dictionary into the output list using a loop.
            flattenOuterDictLoop = Expression.Loop(
                Expression.IfThenElse(
                    Expression.Call(outerDictEnumeratorVariable, ItorMoveNextMethod),
                    Expression.Block(
                        [outerDictEntryParameter],
                        [
                            assignCurrentOuterEntryFromOuterDictEnumerator,
                            addOuterKeyAndValuesToOutput
                        ]),
                    Expression.Break(breakLabel)),
                breakLabel),

            // Disposes the outer dictionary enumerator after flattening.
            disposeOuterDictEnumerator = Expression.Call(
                outerDictEnumeratorVariable,
                DisposeMethod);

        var fullBlock = Expression.Block(
            [outerDictObjEntityVariable, dictInputEnumeratorVariable, outerDictEnumeratorVariable, outputVariable], // Declares variables used in the block
            [
                // Initializes dictObjEntity with a new instance of T
                initializeOuterDict,
                // Sets up the enumerator for the input data list.
                setupInputDataEnumerator,
                // Executes the loop with cleanup
                whileBlock,
                // Initializes the output list with a new instance.
                initializeOutputList,
                // Sets up the enumerator for the outer dictionary to flatten it.
                setupOuterDictEnumerator,
                // Flattens the outer dictionary into the output list using a loop.
                flattenOuterDictLoop,
                // Disposes the outer dictionary enumerator after flattening.
                disposeOuterDictEnumerator,
                // Returns the populated output list.
                outputVariable
            ]);

        // Compiles the expression tree
        var lambda = Expression.Lambda<Func<Dictionary<ITuple, List<TEntity>>>>(fullBlock).Compile();

        // Executes the expression tree, returning the result
        return lambda();
    }
}
