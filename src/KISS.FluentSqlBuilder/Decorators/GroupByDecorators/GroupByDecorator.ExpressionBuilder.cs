namespace KISS.FluentSqlBuilder.Decorators.GroupByDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
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

            // Accesses the inner dictionary in the outer dictionary via the outer key.
            IndexExpression outerKeyAccessor = Expression.MakeIndex(
                    OuterDictObjEntityVariable,
                    OuterDictObjEntityVariable.Type.GetProperty("Item")!,
                    [OuterKeyVariable]),
                // Creates an indexer for accessing or setting dictionary entries (outerDictObjEntity[outerKeyVariable][keyVariable]).
                innerKeyAccessor = Expression.MakeIndex(
                    outerKeyAccessor,
                    InnerDictObjEntityType.GetProperty("Item")!,
                    [InnerKeyVariable]);

            // Generate ValueTuple type dynamically
            var outerKeyConstructor =
                Type.GetType($"{TypeUtils.ValueTupleType.FullName}`{GroupingKeys.Count + AggregationKeys.Count}")!
                    .MakeGenericType([.. GroupingKeys.Values, .. AggregationKeys.Values])
                    .GetConstructor([.. GroupingKeys.Values, .. AggregationKeys.Values])!;

            // Create constructor arguments from currentInputRowParameter
            var outerKeyArguments = GroupingKeys
                .Union(AggregationKeys)
                .Select(grp =>
                    TypeUtils.ChangeType(
                        Expression.Property(CurrentEntryExVariable, "Item", Expression.Constant(grp.Key)),
                        grp.Value))
                .ToArray();

            // Creates and assigns the outer key from grouping data.
            Expression assignOuterKeyVariableFromGroupingData = Expression.Assign(
                    OuterKeyVariable,
                    Expression.Convert(Expression.New(outerKeyConstructor, outerKeyArguments), OuterKeyType)),
                // Sets the inner dictionary key from the row’s "Extend0_Id" value.
                assignInnerKeyVariableFromPrimaryKey = Expression.Assign(
                    InnerKeyVariable,
                    TypeUtils.ChangeType(
                        Expression.Property(CurrentEntryExVariable, "Item", Expression.Constant("Extend0_Id")),
                        InnerKeyType)),
                // Initializes a new inner dictionary if the outer key is missing.
                initializeInnerDictIfOuterKeyMissing = Expression.IfThen(
                    Expression.Not(TypeUtils.IsDictContainsKey(OuterDictObjEntityVariable, OuterKeyVariable)),
                    Expression.Block(
                        [],
                        // Adds the processed entity to the dictionary with its key.
                        TypeUtils.CallMethod(
                            OuterDictObjEntityVariable,
                            "Add",
                            OuterKeyVariable,
                            Expression.New(InnerDictObjEntityType)))),
                // Processes and adds the entity to the inner dictionary if the key is new.
                initializeEntityIfInnerKeyMissing = Expression.IfThen(
                    Expression.Not(TypeUtils.IsDictContainsKey(outerKeyAccessor, InnerKeyVariable)),
                    Expression.Block(
                        [], // Ensures variables is scoped for this operation
                        // Applies the row processor to construct or modify the entity.
                        // IterRowProcessor((CurrentEntryExParameter, CurrentEntityExVariable)),
                        TypeUtils.InitializeTargetValue(
                            CurrentEntityExVariable,
                            TypeUtils.CreateIterRowBindings(
                                CurrentEntryExVariable,
                                InEntityType,
                                CurrentEntityExVariable.Type,
                                GetAliasMapping(InEntityType))),
                        // Adds the processed entity to the dictionary with its key.
                        TypeUtils.CallMethod(outerKeyAccessor, "Add", InnerKeyVariable, CurrentEntityExVariable)));

            // Initializes the outer dictionary with a new instance.
            Expression initializeOuterDict = TypeUtils.InitializeTargetValue(OuterDictObjEntityVariable),
                // Initializes the output list with a new instance.
                initializeOutputList = TypeUtils.InitializeTargetValue(OutputVariable),
                // Sets up the enumerator for the outer dictionary to flatten it.
                setupOuterDictEnumerator = Expression.Assign(
                    OuterDictIterVariable,
                    TypeUtils.CallMethod(OuterDictObjEntityVariable, "GetEnumerator")),
                // Assigns the current entry from the outer dictionary enumerator.
                assignCurrentOuterEntryFromOuterDictEnumerator = Expression.Assign(
                    OuterDictEntryParameter,
                    Expression.Property(OuterDictIterVariable, "Current")),
                // Adds the current key and its inner values as a list to the output.
                addOuterKeyAndValuesToOutput = TypeUtils.CallMethod(
                    OutputVariable,
                    "Add",
                    Expression.Property(OuterDictEntryParameter, "Key"),
                    Expression.New(
                        OutEntitiesType.GetConstructor([OutEntitiesType])!,
                        Expression.Property(Expression.Property(OuterDictEntryParameter, "Value"), "Values"))),
                // Flattens the outer dictionary into the output list using a loop.
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
                // Disposes the outer dictionary enumerator after flattening.
                disposeOuterDictEnumerator = Expression.Call(OuterDictIterVariable, TypeUtils.DisposeMethod);

            var fullBlock = Expression.Block(
                [
                    InEntriesExVariable,
                    OuterDictObjEntityVariable,
                    OuterDictIterVariable,
                    Inner.IndexerExVariable,
                    OutputVariable
                ], // Declares variables used in the block
                [
                    // Initializes dictObjEntity with a new instance of T
                    initializeOuterDict,
                    // Sets up the enumerator for the input data list.
                    // SetupInputDataEnumerator,
                    TypeUtils.InitializeTargetValue(
                        InEntriesExVariable,
                        Expression.Call(InEntriesExParameter, TypeUtils.GetEnumeratorForIEnumDictStrObj)),
                    // Executes the loop with cleanup
                    Expression.TryFinally(
                        Expression.Loop(
                            Expression.IfThenElse(
                                Expression.Call(
                                    InEntriesExVariable,
                                    TypeUtils.IterMoveNextMethod), // If MoveNext returns true (more rows),
                                // ProcessRowsIfExist
                                Expression.Block(
                                    [
                                        CurrentEntryExVariable,
                                        CurrentEntityExVariable,
                                        OuterKeyVariable,
                                        InnerKeyVariable
                                    ], // Local variables for this iteration
                                    [
                                        // Sets the current row from the input data enumerator.
                                        // AssignCurrentInputRowFromInputEnumerator,
                                        TypeUtils.InitializeTargetValue(
                                            CurrentEntryExVariable,
                                            Expression.Property(InEntriesExVariable, "Current")),
                                        // Creates and assigns the outer key from grouping data.
                                        assignOuterKeyVariableFromGroupingData,
                                        // Sets the inner dictionary key from the row’s "Extend0_Id" value.
                                        assignInnerKeyVariableFromPrimaryKey,
                                        // Initializes a new inner dictionary if the outer key is missing.
                                        initializeInnerDictIfOuterKeyMissing,
                                        // Processes and adds the entity to the inner dictionary if the key is new.
                                        initializeEntityIfInnerKeyMissing,
                                        // Applies additional join processors using the dictionary indexer for related data.
                                        TypeUtils.InitializeTargetValue(Inner.IndexerExVariable, innerKeyAccessor),
                                        // .. ApplyJoinProcessorsToInnerKeyAccessor
                                        .. JoinRows
                                    ]),
                                exitsLoop), // Otherwise, break out of the loop
                            breakLabel),
                        Expression.Call(InEntriesExVariable, TypeUtils.DisposeMethod)),
                    // Initializes the output list with a new instance.
                    initializeOutputList,
                    // Sets up the enumerator for the outer dictionary to flatten it.
                    setupOuterDictEnumerator,
                    // Flattens the outer dictionary into the output list using a loop.
                    flattenOuterDictLoop,
                    // Disposes the outer dictionary enumerator after flattening.
                    disposeOuterDictEnumerator,
                    // Returns the populated output list.
                    OutputVariable
                ]);

            return fullBlock;
        }
    }
}
