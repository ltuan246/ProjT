namespace KISS.FluentSqlBuilder.Decorators.GroupByDecorators;

/// <summary>
///     Holds expression variables and logic for constructing and executing grouping operations
///     in the GroupByDecorator. These variables and expressions are used to build LINQ expression trees
///     for SQL GROUP BY transformations, including grouping, aggregation, and output projection.
/// </summary>
public sealed partial record GroupByDecorator
{
    /// <summary>
    ///     Expression variable for the output dictionary that collects grouped results.
    /// </summary>
    public ParameterExpression OutputVariable { get; init; }

    /// <summary>
    ///     Expression variable for the outer dictionary that maps group keys to inner dictionaries.
    /// </summary>
    public ParameterExpression OuterDictObjEntityVariable { get; init; }

    /// <summary>
    ///     Expression variable representing the group key for the outer dictionary.
    /// </summary>
    public ParameterExpression OuterKeyVariable { get; init; }

    /// <summary>
    ///     Expression variable representing the aggregation key for the inner dictionary.
    /// </summary>
    public ParameterExpression InnerKeyVariable { get; init; }

    /// <summary>
    ///     Expression variable used as an enumerator for iterating the outer dictionary.
    /// </summary>
    public ParameterExpression OuterDictIterVariable { get; init; }

    /// <summary>
    ///     Expression parameter representing the current entry in the outer dictionary during iteration.
    /// </summary>
    public ParameterExpression OuterDictEntryParameter { get; init; }

    /// <inheritdoc />
    public override BlockExpression Block
    {
        get
        {
            // Label for breaking out of loops.
            var breakLabel = Expression.Label();
            var exitsLoop = Expression.Break(breakLabel);

            // Access the inner dictionary in the outer dictionary using the group key.
            IndexExpression outerKeyAccessor = Expression.MakeIndex(
                    OuterDictObjEntityVariable,
                    OuterDictObjEntityVariable.Type.GetProperty("Item")!,
                    [OuterKeyVariable]),

                // Access or set entries in the inner dictionary using the aggregation key.
                innerKeyAccessor = Expression.MakeIndex(
                    outerKeyAccessor,
                    InnerDictObjEntityType.GetProperty("Item")!,
                    [InnerKeyVariable]);

            // Gather type arguments for the group key tuple.
            Type[] typeArguments = [.. GroupingKeys.Values, .. AggregationKeys.Values];

            // Dynamically create the ValueTuple type for the group key.
            var outerKeyConstructor =
                Type.GetType($"{TypeUtils.ValueTupleType.FullName}`{typeArguments.Length}")!
                    .MakeGenericType(typeArguments)
                    .GetConstructor(typeArguments)!;

            // Build constructor arguments for the group key from the current input row.
            var outerKeyArguments = GroupingKeys
                .Union(AggregationKeys)
                .Select(grp =>
                    TypeUtils.ChangeType(
                        Expression.Property(CurrentEntryExVariable, "Item", Expression.Constant(grp.Key)),
                        grp.Value))
                .ToArray();

            // Find primary key properties for the entity.
            var primaryKeys = InEntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.GetCustomAttributes(typeof(KeyBuilderAttribute), true).Length != 0)
                .ToArray();

            var primaryKeyTypes = primaryKeys.Select(pI => pI.PropertyType).ToArray();

            // Dynamically create the ValueTuple type for the aggregation key.
            var innerKeyConstructor =
                Type.GetType($"{TypeUtils.ValueTupleType.FullName}`{primaryKeys.Length}")!
                    .MakeGenericType(primaryKeyTypes)
                    .GetConstructor(primaryKeyTypes)!;

            // Build constructor arguments for the aggregation key from the current input row.
            var innerKeyArguments = primaryKeys
                .Select(k =>
                    TypeUtils.ChangeType(
                        Expression.Property(CurrentEntryExVariable, "Item", Expression.Constant($"Extend0_{k.Name}")),
                        k.PropertyType))
                .ToArray();

            // Assign the group key variable from grouping data.
            Expression assignOuterKeyVariableFromGroupingData = Expression.Assign(
                    OuterKeyVariable,
                    Expression.Convert(Expression.New(outerKeyConstructor, outerKeyArguments), TypeUtils.TupleType)),

                // Assign the aggregation key variable from the primary key.
                assignInnerKeyVariableFromPrimaryKey = Expression.Assign(
                    InnerKeyVariable,
                    Expression.Convert(Expression.New(innerKeyConstructor, innerKeyArguments), TypeUtils.TupleType)),

                // If the group key is missing, initialize a new inner dictionary for it.
                initializeInnerDictIfOuterKeyMissing = Expression.IfThen(
                    Expression.Not(TypeUtils.IsDictContainsKey(OuterDictObjEntityVariable, OuterKeyVariable)),
                    Expression.Block(
                        [],
                        // Add a new inner dictionary for the missing group key.
                        TypeUtils.CallMethod(
                            OuterDictObjEntityVariable,
                            "Add",
                            OuterKeyVariable,
                            Expression.New(InnerDictObjEntityType)))),
                // If the aggregation key is missing, initialize and add a new entity to the inner dictionary.
                initializeEntityIfInnerKeyMissing = Expression.IfThen(
                    Expression.Not(TypeUtils.IsDictContainsKey(outerKeyAccessor, InnerKeyVariable)),
                    Expression.Block(
                        [], // Local scope
                        // Initialize the entity from the current row.
                        TypeUtils.InitializeTargetValue(
                            CurrentEntityExVariable,
                            TypeUtils.CreateIterRowBindings(
                                CurrentEntryExVariable,
                                InEntityType,
                                CurrentEntityExVariable.Type,
                                GetAliasMapping(InEntityType))),
                        // Add the new entity to the inner dictionary.
                        TypeUtils.CallMethod(outerKeyAccessor, "Add", InnerKeyVariable, CurrentEntityExVariable)));

            // Initialize the outer dictionary variable.
            Expression initializeOuterDict = TypeUtils.InitializeTargetValue(OuterDictObjEntityVariable),
                // Initialize the output list variable.
                initializeOutputList = TypeUtils.InitializeTargetValue(OutputVariable),
                // Set up the enumerator for the outer dictionary.
                setupOuterDictEnumerator = Expression.Assign(
                    OuterDictIterVariable,
                    TypeUtils.CallMethod(OuterDictObjEntityVariable, "GetEnumerator")),
                // Assign the current entry from the outer dictionary enumerator.
                assignCurrentOuterEntryFromOuterDictEnumerator = Expression.Assign(
                    OuterDictEntryParameter,
                    Expression.Property(OuterDictIterVariable, "Current")),
                // Add the current group key and its values as a list to the output.
                addOuterKeyAndValuesToOutput = TypeUtils.CallMethod(
                    OutputVariable,
                    "Add",
                    Expression.Property(OuterDictEntryParameter, "Key"),
                    Expression.New(
                        OutEntitiesType.GetConstructor([OutEntitiesType])!,
                        Expression.Property(Expression.Property(OuterDictEntryParameter, "Value"), "Values"))),
                // Loop to flatten the outer dictionary into the output list.
                flattenOuterDictLoop = Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Call(OuterDictIterVariable, TypeUtils.IterMoveNextMethod),
                        Expression.Block(
                            [OuterDictEntryParameter],
                            [
                                assignCurrentOuterEntryFromOuterDictEnumerator,
                                addOuterKeyAndValuesToOutput
                            ]),
                        exitsLoop),
                    breakLabel),
                // Dispose the outer dictionary enumerator after flattening.
                disposeOuterDictEnumerator = Expression.Call(OuterDictIterVariable, TypeUtils.DisposeMethod);

            // Compose the full block expression for grouping and output projection.
            var fullBlock = Expression.Block(
                [
                    InEntriesExVariable,
                    OuterDictObjEntityVariable,
                    OuterDictIterVariable,
                    Inner.IndexerExVariable,
                    OutputVariable
                ], // Declare variables used in the block
                [
                    // Initialize the outer dictionary.
                    initializeOuterDict,
                    // Initialize the enumerator for the input data list.
                    TypeUtils.InitializeTargetValue(
                        InEntriesExVariable,
                        Expression.Call(InEntriesExParameter, TypeUtils.GetEnumeratorForIEnumDictStrObj)),
                    // Main processing loop with cleanup.
                    Expression.TryFinally(
                        Expression.Loop(
                            Expression.IfThenElse(
                                Expression.Call(
                                    InEntriesExVariable,
                                    TypeUtils.IterMoveNextMethod), // If more rows exist,
                                // Process the current row.
                                Expression.Block(
                                    [
                                        CurrentEntryExVariable,
                                        CurrentEntityExVariable,
                                        OuterKeyVariable,
                                        InnerKeyVariable
                                    ], // Local variables for this iteration
                                    [
                                        // Assign the current row from the input enumerator.
                                        TypeUtils.InitializeTargetValue(
                                            CurrentEntryExVariable,
                                            Expression.Property(InEntriesExVariable, "Current")),
                                        // Assign the group key from grouping data.
                                        assignOuterKeyVariableFromGroupingData,
                                        // Assign the aggregation key from the primary key.
                                        assignInnerKeyVariableFromPrimaryKey,
                                        // Initialize a new inner dictionary if needed.
                                        initializeInnerDictIfOuterKeyMissing,
                                        // Initialize and add a new entity if needed.
                                        initializeEntityIfInnerKeyMissing,
                                        // Set the indexer for join processing.
                                        TypeUtils.InitializeTargetValue(Inner.IndexerExVariable, innerKeyAccessor),
                                        // Apply join processors to the inner key accessor.
                                        .. JoinRows
                                    ]),
                                exitsLoop), // Otherwise, exit the loop
                            breakLabel),
                        Expression.Call(InEntriesExVariable, TypeUtils.DisposeMethod)),
                    // Initialize the output list.
                    initializeOutputList,
                    // Set up the enumerator for the outer dictionary.
                    setupOuterDictEnumerator,
                    // Flatten the outer dictionary into the output list.
                    flattenOuterDictLoop,
                    // Dispose the outer dictionary enumerator.
                    disposeOuterDictEnumerator,
                    // Return the populated output list.
                    OutputVariable
                ]);

            return fullBlock;
        }
    }
}
