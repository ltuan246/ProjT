namespace KISS.FluentSqlBuilder.Decorators.GroupByDecorators;

/// <summary>
///     Provides the expression tree logic for grouping and aggregating entities in the GroupByDecorator.
///     This class builds the LINQ expression block that performs grouping, aggregation, and output
///     transformation for SQL GROUP BY operations, using dynamically constructed expression variables.
/// </summary>
public sealed partial record GroupByDecorator
{
    /// <inheritdoc />
    public override BlockExpression Block
    {
        get
        {
            var breakLabel = Expression.Label();
            var exitsLoop = Expression.Break(breakLabel);

            // Access the inner dictionary in the outer dictionary using the outer key.
            IndexExpression outerKeyAccessor = Expression.MakeIndex(
                    OuterDictObjEntityVariable,
                    OuterDictObjEntityVariable.Type.GetProperty("Item")!,
                    [OuterKeyVariable]),
                // Access or set entries in the inner dictionary (outerDictObjEntity[outerKeyVariable][keyVariable]).
                innerKeyAccessor = Expression.MakeIndex(
                    outerKeyAccessor,
                    InnerDictObjEntityType.GetProperty("Item")!,
                    [InnerKeyVariable]);

            // Dynamically generate the ValueTuple type for the group key.
            var outerKeyConstructor =
                Type.GetType($"{TypeUtils.ValueTupleType.FullName}`{GroupingKeys.Count + AggregationKeys.Count}")!
                    .MakeGenericType([.. GroupingKeys.Values, .. AggregationKeys.Values])
                    .GetConstructor([.. GroupingKeys.Values, .. AggregationKeys.Values])!;

            // Create constructor arguments for the group key from the current input row.
            var outerKeyArguments = GroupingKeys
                .Union(AggregationKeys)
                .Select(grp =>
                    TypeUtils.ChangeType(
                        Expression.Property(CurrentEntryExVariable, "Item", Expression.Constant(grp.Key)),
                        grp.Value))
                .ToArray();

            // Assign the outer key variable from grouping data.
            Expression assignOuterKeyVariableFromGroupingData = Expression.Assign(
                    OuterKeyVariable,
                    Expression.Convert(Expression.New(outerKeyConstructor, outerKeyArguments), OuterKeyType)),
                // Assign the inner key variable from the primary key ("Extend0_Id") in the row.
                assignInnerKeyVariableFromPrimaryKey = Expression.Assign(
                    InnerKeyVariable,
                    TypeUtils.ChangeType(
                        Expression.Property(CurrentEntryExVariable, "Item", Expression.Constant("Extend0_Id")),
                        InnerKeyType)),
                // Initialize a new inner dictionary if the outer key is not present.
                initializeInnerDictIfOuterKeyMissing = Expression.IfThen(
                    Expression.Not(TypeUtils.IsDictContainsKey(OuterDictObjEntityVariable, OuterKeyVariable)),
                    Expression.Block(
                        [],
                        // Add a new inner dictionary for the missing outer key.
                        TypeUtils.CallMethod(
                            OuterDictObjEntityVariable,
                            "Add",
                            OuterKeyVariable,
                            Expression.New(InnerDictObjEntityType)))),
                // Initialize and add a new entity to the inner dictionary if the inner key is not present.
                initializeEntityIfInnerKeyMissing = Expression.IfThen(
                    Expression.Not(TypeUtils.IsDictContainsKey(outerKeyAccessor, InnerKeyVariable)),
                    Expression.Block(
                        [], // Scope for local variables
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
                                        // Assign the inner key from the primary key.
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
