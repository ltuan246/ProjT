namespace KISS.FluentSqlBuilder.Core;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
/// <typeparam name="TRecordset">The type representing the database table or view.</typeparam>
/// <typeparam name="TReturn">
///     The type of objects to return, representing the query result rows.
///     This type must match the structure of the query results.
/// </typeparam>
public sealed partial record QueryOperator<TRecordset, TReturn>
{
    /// <summary>
    ///     Executes the constructed SQL query against the database and returns the results
    ///     as a list of the specified type. This method handles both simple and complex
    ///     query scenarios, automatically selecting the appropriate processing strategy.
    /// </summary>
    /// <typeparam name="TReturn">
    ///     The type of objects to return, representing the query result rows.
    ///     This type must match the structure of the query results.
    /// </typeparam>
    /// <returns>
    ///     A list of <typeparamref name="TReturn" /> objects retrieved based on the query conditions.
    ///     The list is empty if no results are found.
    /// </returns>
    public List<TReturn> GetList()
    {
        // Compiles the expression tree
        var lambda = Expression
            .Lambda<Func<List<IDictionary<string, object>>, List<TReturn>>>(
                Composite.Block,
                Composite.InEntriesExParameter)
            .Compile();

        // Executes the expression tree, returning the result
        return lambda(InputData);
    }

    /// <summary>
    ///     Executes the constructed SQL query against the database and returns the results
    ///     as a dictionary with composite keys and lists of the specified type. This method
    ///     is designed for handling complex queries with multiple result sets or nested data.
    /// </summary>
    /// <typeparam name="TReturn">
    ///     The type of objects to return, representing the query result rows.
    ///     This type must match the structure of the query results.
    /// </typeparam>
    /// <returns>
    ///     A dictionary where the key is a composite tuple representing unique identifiers,
    ///     and the value is a list of <typeparamref name="TReturn" /> objects associated with
    ///     that key. The dictionary is empty if no results are found.
    /// </returns>
    public Dictionary<ITuple, List<TReturn>> GetDictionary()
    {
        // Compiles the expression tree
        var lambda = Expression
            .Lambda<Func<List<IDictionary<string, object>>, Dictionary<ITuple, List<TReturn>>>>(
                Composite.Block,
                Composite.InEntriesExParameter)
            .Compile();

        // Executes the expression tree, returning the result
        return lambda(InputData);

        // // Defines the type of the final output collection
        // Type returnType = typeof(Dictionary<ITuple, List<TReturn>>),
        //     // Defines the type of outer dictionary for the intermediate dictionary used for uniqueness.
        //     // Specifies the type of the key used in the dictionary, extracted from each row for indexing.
        //     outerKeyType = typeof(ITuple),
        //     // Defines the type of the intermediate dictionary used for uniqueness.
        //     dictObjEntityType = typeof(Dictionary<object, TReturn>),
        //     // Specifies the type of the key used in the dictionary, extracted from each row for indexing.
        //     dictKeyType = typeof(object),
        //     outerDictObjEntityType = typeof(Dictionary<ITuple, Dictionary<object, TReturn>>),
        //     outerIterType = typeof(Dictionary<ITuple, Dictionary<object, TReturn>>.Enumerator),
        //     outerEntryType = typeof(KeyValuePair<ITuple, Dictionary<object, TReturn>>);

        // // Collection that accumulates all processed entities
        // ParameterExpression outputVariable = Expression.Variable(returnType, "outputVariable"),
        //     // Intermediate dictionary that ensures uniqueness of entities by key
        //     // Key extracted from each row for dictionary indexing
        //     outerKeyVariable = Expression.Variable(outerKeyType, "outerKeyVariable"),
        //     innerKeyVariable = Expression.Variable(dictKeyType, "keyVariable"),
        //     // Enumerator for iterating over the outer dictionary of entities
        //     outerDictObjEntityVariable = Expression.Variable(outerDictObjEntityType, "outerDictObjEntityVariable"),
        //     outerDictIterVariable = Expression.Variable(outerIterType, "outerDictIterVariable"),
        //     // Current key-value pair from the outer dictionary being processed
        //     outerDictEntryParameter = Expression.Parameter(outerEntryType, "outerDictEntryParameter");

        // // Accesses the inner dictionary in the outer dictionary via the outer key.
        // IndexExpression outerKeyAccessor = Expression.MakeIndex(
        //         outerDictObjEntityVariable,
        //         outerDictObjEntityType.GetProperty("Item")!,
        //         [outerKeyVariable]),
        //     // Creates an indexer for accessing or setting dictionary entries (outerDictObjEntity[outerKeyVariable][keyVariable]).
        //     innerKeyAccessor = Expression.MakeIndex(
        //         outerKeyAccessor,
        //         dictObjEntityType.GetProperty("Item")!,
        //         [innerKeyVariable]);

        // // Generate ValueTuple type dynamically
        // var outerKeyConstructor =
        //     Type.GetType($"{TypeUtils.ValueTupleType.FullName}`{Composite.GroupingKeys.Count + Composite.AggregationKeys.Count}")!
        //         .MakeGenericType([.. Composite.GroupingKeys.Values, .. Composite.AggregationKeys.Values])
        //         .GetConstructor([.. Composite.GroupingKeys.Values, .. Composite.AggregationKeys.Values])!;

        // // Create constructor arguments from currentInputRowParameter
        // var outerKeyArguments = Composite.GroupingKeys
        //     .Union(Composite.AggregationKeys)
        //     .Select(grp =>
        //         TypeUtils.ChangeType(
        //             Expression.Property(Composite.CurrentEntryExParameter, "Item", Expression.Constant(grp.Key)),
        //             grp.Value))
        //     .ToArray();

        // // Creates and assigns the outer key from grouping data.
        // Expression assignOuterKeyVariableFromGroupingData = Expression.Assign(
        //         outerKeyVariable,
        //         Expression.Convert(Expression.New(outerKeyConstructor, outerKeyArguments), outerKeyType)),
        //     // Sets the inner dictionary key from the row’s "Extend0_Id" value.
        //     assignInnerKeyVariableFromPrimaryKey = Expression.Assign(
        //         innerKeyVariable,
        //         TypeUtils.ChangeType(
        //             Expression.Property(Composite.CurrentEntryExParameter, "Item", Expression.Constant("Extend0_Id")),
        //             dictKeyType)),
        //     // Initializes a new inner dictionary if the outer key is missing.
        //     initializeInnerDictIfOuterKeyMissing = Expression.IfThen(
        //         Expression.Not(TypeUtils.IsDictContainsKey(outerDictObjEntityVariable, outerKeyVariable)),
        //         Expression.Block(
        //             [],
        //             // Adds the processed entity to the dictionary with its key.
        //             TypeUtils.CallMethod(outerDictObjEntityVariable, "Add", outerKeyVariable, Expression.New(dictObjEntityType)))),
        //     // Processes and adds the entity to the inner dictionary if the key is new.
        //     initializeEntityIfInnerKeyMissing = Expression.IfThen(
        //         Expression.Not(TypeUtils.IsDictContainsKey(outerKeyAccessor, innerKeyVariable)),
        //         Expression.Block(
        //             [], // Ensures variables is scoped for this operation
        //                 // Applies the row processor to construct or modify the entity.
        //                 // IterRowProcessor((CurrentEntryExParameter, CurrentEntityExVariable)),
        //             TypeUtils.InitializeTargetValue(
        //                 Composite.CurrentEntityExVariable,
        //                 TypeUtils.CreateIterRowBindings(
        //                     Composite.CurrentEntryExParameter,
        //                     Composite.InEntityType!,
        //                     Composite.CurrentEntityExVariable.Type,
        //                     Composite.GetAliasMapping(Composite.InEntityType!))),
        //             // Adds the processed entity to the dictionary with its key.
        //             TypeUtils.CallMethod(outerKeyAccessor, "Add", innerKeyVariable, Composite.CurrentEntityExVariable)));

        // // Initializes the outer dictionary with a new instance.
        // Expression initializeOuterDict = TypeUtils.InitializeTargetValue(outerDictObjEntityVariable),
        //     // Initializes the output list with a new instance.
        //     initializeOutputList = TypeUtils.InitializeTargetValue(outputVariable),
        //     // Sets up the enumerator for the outer dictionary to flatten it.
        //     setupOuterDictEnumerator = Expression.Assign(
        //         outerDictIterVariable,
        //         TypeUtils.CallMethod(outerDictObjEntityVariable, "GetEnumerator")),
        //     // Assigns the current entry from the outer dictionary enumerator.
        //     assignCurrentOuterEntryFromOuterDictEnumerator = Expression.Assign(
        //         outerDictEntryParameter,
        //         Expression.Property(outerDictIterVariable, "Current")),
        //     // Adds the current key and its inner values as a list to the output.
        //     addOuterKeyAndValuesToOutput = TypeUtils.CallMethod(
        //         outputVariable,
        //         "Add",
        //         Expression.Property(outerDictEntryParameter, "Key"),
        //         Expression.New(
        //             typeof(List<TReturn>).GetConstructor([typeof(IEnumerable<TReturn>)])!,
        //             Expression.Property(Expression.Property(outerDictEntryParameter, "Value"), "Values"))),
        //     // Flattens the outer dictionary into the output list using a loop.
        //     flattenOuterDictLoop = Expression.Loop(
        //         Expression.IfThenElse(
        //             Expression.Call(outerDictIterVariable, TypeUtils.IterMoveNextMethod),
        //             Expression.Block(
        //                 [outerDictEntryParameter],
        //                 [
        //                     assignCurrentOuterEntryFromOuterDictEnumerator,
        //                     addOuterKeyAndValuesToOutput
        //                 ]),
        //             ExitsLoop),
        //         BreakLabel),
        //     // Disposes the outer dictionary enumerator after flattening.
        //     disposeOuterDictEnumerator = Expression.Call(outerDictIterVariable, TypeUtils.DisposeMethod);

        // var fullBlock = Expression.Block(
        //     [
        //         Composite.InEntryIterExVariable,
        //         outerDictObjEntityVariable,
        //         outerDictIterVariable,
        //         outputVariable
        //     ], // Declares variables used in the block
        //     [
        //         // Initializes dictObjEntity with a new instance of T
        //         initializeOuterDict,
        //         // Sets up the enumerator for the input data list.
        //         // SetupInputDataEnumerator,
        //         Expression.Assign(
        //             Composite.InEntryIterExVariable,
        //             Expression.Call(Expression.Constant(InputData), TypeUtils.GetEnumeratorForIEnumDictStrObj)),
        //         // Executes the loop with cleanup
        //         Expression.TryFinally(
        //             Expression.Loop(
        //                 Expression.IfThenElse(
        //                     Expression.Call(Composite.InEntryIterExVariable, TypeUtils.IterMoveNextMethod), // If MoveNext returns true (more rows),
        //                     // ProcessRowsIfExist
        //                     Expression.Block(
        //                         [
        //                             Composite.CurrentEntryExParameter,
        //                             Composite.CurrentEntityExVariable,
        //                             outerKeyVariable,
        //                             innerKeyVariable
        //                         ], // Local variables for this iteration
        //                         [
        //                             // Sets the current row from the input data enumerator.
        //                             // AssignCurrentInputRowFromInputEnumerator,
        //                             Expression.Assign(
        //                                 Composite.CurrentEntryExParameter,
        //                                 Expression.Property(Composite.InEntryIterExVariable, "Current")),
        //                             // Creates and assigns the outer key from grouping data.
        //                             assignOuterKeyVariableFromGroupingData,
        //                             // Sets the inner dictionary key from the row’s "Extend0_Id" value.
        //                             assignInnerKeyVariableFromPrimaryKey,
        //                             // Initializes a new inner dictionary if the outer key is missing.
        //                             initializeInnerDictIfOuterKeyMissing,
        //                             // Processes and adds the entity to the inner dictionary if the key is new.
        //                             initializeEntityIfInnerKeyMissing,
        //                             // Applies additional join processors using the dictionary indexer for related data.
        //                             // .. ApplyJoinProcessorsToInnerKeyAccessor
        //                             .. Composite.JoinRowProcessors
        //                         ]),
        //                     ExitsLoop), // Otherwise, break out of the loop
        //                 BreakLabel),
        //             Expression.Call(Composite.InEntryIterExVariable, TypeUtils.DisposeMethod)),
        //         // Initializes the output list with a new instance.
        //         initializeOutputList,
        //         // Sets up the enumerator for the outer dictionary to flatten it.
        //         setupOuterDictEnumerator,
        //         // Flattens the outer dictionary into the output list using a loop.
        //         flattenOuterDictLoop,
        //         // Disposes the outer dictionary enumerator after flattening.
        //         disposeOuterDictEnumerator,
        //         // Returns the populated output list.
        //         outputVariable
        //     ]);

        // // Compiles the expression tree
        // var lambda = Expression.Lambda<Func<Dictionary<ITuple, List<TReturn>>>>(fullBlock).Compile();

        // // Executes the expression tree, returning the result
        // return lambda();
    }
}
