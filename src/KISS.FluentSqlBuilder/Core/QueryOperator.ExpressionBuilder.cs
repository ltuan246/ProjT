namespace KISS.FluentSqlBuilder.Core;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
/// <typeparam name="TReturn">
///     The type of objects to return, representing the query result rows.
///     This type must match the structure of the query results.
/// </typeparam>
public sealed partial record QueryOperator<TReturn>
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
        (List<ParameterExpression> variables, List<Expression> expressions) = ([], []);
        List<Expression> joinRowProcessors = [];

        var lastHandler = ChainHandler;
        while (lastHandler.NextHandler is not null)
        {
            switch (lastHandler)
            {
                case ISelectHandler sh:
                    InEntityType = sh.InEntityType;
                    CurrentEntityExVariable = sh.CurrentEntityExVariable;
                    OutEntitiesExVariable = sh.OutEntitiesExVariable;

                    (variables, expressions) =
                        ([
                                InEntryIterExVariable,
                                OutEntitiesExVariable
                            ], // Declares variables used in the block
                            [
                                // Initializes outputCollection with a new instance of T
                                InitializeOutputVariable,
                                // Sets up the enumerator for inputData
                                SetupInputDataEnumerator,
                                // Executes the loop with cleanup
                                Expression.TryFinally(
                                    Expression.Loop(
                                        Expression.IfThenElse(
                                            MoveNextOnInEntryEnumerator, // If MoveNext returns true (more rows),
                                            Expression.Block(
                                                [
                                                    CurrentEntryExParameter,
                                                    CurrentEntityExVariable
                                                ],
                                                [
                                                    // Execute the loop body with the current row
                                                    AssignCurrentInputRowFromInputEnumerator,
                                                    // InitializeEntityIfKeyMissing
                                                    Expression.IfThen(
                                                        Expression.Constant(true),
                                                        Expression.Block(
                                                            [], // Ensures variables is scoped for this operation
                                                            // Applies the row processor to construct or modify the entity.
                                                            Composite.InitializeTargetValueBlock(
                                                                CurrentEntityExVariable,
                                                                Composite.CurrentEntryExParameter,
                                                                InEntityType,
                                                                OutEntityType),
                                                            // Adds the processed entity to the dictionary with its key.
                                                            TypeUtils.CallMethod(
                                                                OutEntitiesExVariable, // Calls the Add method on the output list.
                                                                "Add",
                                                                CurrentEntityExVariable))),
                                                ]),
                                            ExitsLoop), // Otherwise, break out of the loop
                                        BreakLabel),
                                    DisposeInEntryEnumerator),
                                // Returns the populated collection
                                OutEntitiesExVariable
                            ]);

                    break;

                case IJoinHandler sh:
                    joinRowProcessors.Add(sh.JoinRowBlock);
                    break;

                default:
                    break;
            }

            lastHandler = lastHandler.NextHandler;
        }

        if (joinRowProcessors.Count != 0)
        {
            ConstantExpression primaryKey = Expression.Constant("Extend0_Id");

            (variables, expressions) =
                ([
                    InEntryIterExVariable,
                    OutEntitiesExVariable,
                    OutDictEntityTypeExVariable,
                ], // Declares variables used in the block
                [
                    // Initialize outputList with a new list
                    InitializeOutputVariable,
                    // Sets up the enumerator for inputData
                    SetupInputDataEnumerator,

                    // InitializeDictVariable: Initializes dictObjEntity with a new instance of T
                    Expression.Assign(
                        OutDictEntityTypeExVariable,
                        Expression.New(OutDictEntityTypeExVariable.Type)),
                    // Executes the loop with cleanup
                    Expression.TryFinally(
                        Expression.Loop(
                            Expression.IfThenElse(
                                MoveNextOnInEntryEnumerator, // If MoveNext returns true (more rows),
                                Expression.Block(
                                    [
                                        CurrentEntryExParameter,
                                        CurrentEntityExVariable,
                                        OutDictKeyExVariable!
                                    ],
                                    [
                                        // Assigns the current row from the enumerator to iterationRowParameter.
                                        AssignCurrentInputRowFromInputEnumerator,
                                        // Extracts and converts the "Extend0_Id" key from the row to a string.
                                        Expression.Assign(
                                            OutDictKeyExVariable!,
                                            TypeUtils.ChangeType(
                                                Expression.Property(CurrentEntryExParameter, "Item", primaryKey),
                                                TypeUtils.ObjType)),
                                        // InitializeEntityIfKeyMissing: Processes the row if its key isn’t already in the dictionary.
                                        Expression.IfThen(
                                            Expression.Not(TypeUtils.IsDictContainsKey(OutDictEntityTypeExVariable, OutDictKeyExVariable!)),
                                            Expression.Block(
                                                [], // Ensures variables is scoped for this operation
                                                // Applies the row processor to construct or modify the entity.
                                                Composite.InitializeTargetValueBlock(
                                                    CurrentEntityExVariable,
                                                    Composite.CurrentEntryExParameter,
                                                    InEntityType!,
                                                    OutEntityType),
                                                // Adds the processed entity to the dictionary with its key.
                                                TypeUtils.CallMethod(OutDictEntityTypeExVariable, "Add", OutDictKeyExVariable!, CurrentEntityExVariable))),
                                        // Applies additional join processors using the dictionary indexer for related data.
                                        // .. ApplyJoinProcessorsToInnerKeyAccessor
                                        .. joinRowProcessors
                                    ]),
                                ExitsLoop), // Otherwise, break out of the loop
                            BreakLabel),
                        DisposeInEntryEnumerator),

                    // Convert dictionary values to list
                    TypeUtils.CallMethod(
                        OutEntitiesExVariable,
                        "AddRange",
                        Expression.Property(OutDictEntityTypeExVariable, "Values")),

                    // Return the populated list
                    OutEntitiesExVariable
                ]);
        }

        var fullBlock = Expression.Block(variables, expressions);

        // Compiles the expression tree
        var lambda = Expression.Lambda<Func<List<TReturn>>>(fullBlock).Compile();

        // Executes the expression tree, returning the result
        return lambda();
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
        var lastHandler = ChainHandler;
        while (lastHandler.NextHandler is not null)
        {
            switch (lastHandler)
            {
                case ISelectHandler sh:
                    InEntityType = sh.InEntityType;
                    CurrentEntityExVariable = sh.CurrentEntityExVariable;
                    OutEntitiesExVariable = sh.OutEntitiesExVariable;
                    break;
            }

            lastHandler = lastHandler.NextHandler;
        }

        // Defines the type of the final output collection
        Type returnType = typeof(Dictionary<ITuple, List<TReturn>>),
            // Defines the type of outer dictionary for the intermediate dictionary used for uniqueness.
            outerDictObjEntityType = typeof(Dictionary<ITuple, Dictionary<object, TReturn>>),
            // Specifies the type of the key used in the dictionary, extracted from each row for indexing.
            outerKeyType = typeof(ITuple),
            // Defines the type of the intermediate dictionary used for uniqueness.
            dictObjEntityType = typeof(Dictionary<object, TReturn>),
            // Specifies the type of the key used in the dictionary, extracted from each row for indexing.
            dictKeyType = typeof(object),
            outerIterType = typeof(Dictionary<ITuple, Dictionary<object, TReturn>>.Enumerator),
            outerEntryType = typeof(KeyValuePair<ITuple, Dictionary<object, TReturn>>);

        // Collection that accumulates all processed entities
        ParameterExpression outputVariable = Expression.Variable(returnType, "outputVariable"),
            // Intermediate dictionary that ensures uniqueness of entities by key
            outerDictObjEntityVariable = Expression.Variable(outerDictObjEntityType, "outerDictObjEntityVariable"),
            // Key extracted from each row for dictionary indexing
            outerKeyVariable = Expression.Variable(outerKeyType, "outerKeyVariable"),
            innerKeyVariable = Expression.Variable(dictKeyType, "keyVariable"),
            // Enumerator for iterating over the outer dictionary of entities
            outerDictIterVariable = Expression.Variable(outerIterType, "outerDictIterVariable"),
            // Current key-value pair from the outer dictionary being processed
            outerDictEntryParameter = Expression.Parameter(outerEntryType, "outerDictEntryParameter");

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
        var outerKeyConstructor =
            Type.GetType($"{TypeUtils.ValueTupleType.FullName}`{Composite.GroupingKeys.Count + Composite.AggregationKeys.Count}")!
                .MakeGenericType([.. Composite.GroupingKeys.Values, .. Composite.AggregationKeys.Values])
                .GetConstructor([.. Composite.GroupingKeys.Values, .. Composite.AggregationKeys.Values])!;

        // Create constructor arguments from currentInputRowParameter
        var outerKeyArguments = Composite.GroupingKeys
            .Union(Composite.AggregationKeys)
            .Select(grp =>
                TypeUtils.ChangeType(
                    Expression.Property(CurrentEntryExParameter, "Item", Expression.Constant(grp.Key)),
                    grp.Value))
            .ToArray();

        // Creates and assigns the outer key from grouping data.
        Expression assignOuterKeyVariableFromGroupingData = Expression.Assign(
                outerKeyVariable,
                Expression.Convert(Expression.New(outerKeyConstructor, outerKeyArguments), outerKeyType)),

            // Sets the inner dictionary key from the row’s "Extend0_Id" value.
            assignInnerKeyVariableFromPrimaryKey = Expression.Assign(
                innerKeyVariable,
                TypeUtils.ChangeType(
                    Expression.Property(CurrentEntryExParameter, "Item", Expression.Constant("Extend0_Id")),
                    dictKeyType)),

            // Initializes a new inner dictionary if the outer key is missing.
            initializeInnerDictIfOuterKeyMissing = Expression.IfThen(
                Expression.Not(TypeUtils.IsDictContainsKey(outerDictObjEntityVariable, outerKeyVariable)),
                Expression.Block(
                    [],
                    // Adds the processed entity to the dictionary with its key.
                    TypeUtils.CallMethod(outerDictObjEntityVariable, "Add", outerKeyVariable, Expression.New(dictObjEntityType)))),

            // Processes and adds the entity to the inner dictionary if the key is new.
            initializeEntityIfInnerKeyMissing = Expression.IfThen(
                Expression.Not(TypeUtils.IsDictContainsKey(outerKeyAccessor, innerKeyVariable)),
                Expression.Block(
                    [], // Ensures variables is scoped for this operation
                        // Applies the row processor to construct or modify the entity.
                        // IterRowProcessor((CurrentEntryExParameter, CurrentEntityExVariable)),
                    Composite.InitializeTargetValueBlock(
                        CurrentEntityExVariable,
                        Composite.CurrentEntryExParameter,
                        InEntityType!,
                        OutEntityType),
                    // Adds the processed entity to the dictionary with its key.
                    TypeUtils.CallMethod(outerKeyAccessor, "Add", innerKeyVariable, CurrentEntityExVariable)));

        // Initializes the outer dictionary with a new instance.
        Expression initializeOuterDict = Expression.Assign(
                outerDictObjEntityVariable,
                Expression.New(outerDictObjEntityType)),

            // Initializes the output list with a new instance.
            initializeOutputList = Expression.Assign(
                outputVariable,
                Expression.New(returnType)),

            // Sets up the enumerator for the outer dictionary to flatten it.
            setupOuterDictEnumerator = Expression.Assign(
                outerDictIterVariable,
                Expression.Call(outerDictObjEntityVariable, outerDictObjEntityType.GetMethod("GetEnumerator")!)),

            // Assigns the current entry from the outer dictionary enumerator.
            assignCurrentOuterEntryFromOuterDictEnumerator = Expression.Assign(
                outerDictEntryParameter,
                Expression.Property(outerDictIterVariable, "Current")),

            // Adds the current key and its inner values as a list to the output.
            addOuterKeyAndValuesToOutput = TypeUtils.CallMethod(
                outputVariable,
                "Add",
                Expression.Property(outerDictEntryParameter, "Key"),
                Expression.New(
                    typeof(List<TReturn>).GetConstructor([typeof(IEnumerable<TReturn>)])!,
                    Expression.Property(Expression.Property(outerDictEntryParameter, "Value"), "Values"))),

            // Flattens the outer dictionary into the output list using a loop.
            flattenOuterDictLoop = Expression.Loop(
                Expression.IfThenElse(
                    Expression.Call(outerDictIterVariable, TypeUtils.IterMoveNextMethod),
                    Expression.Block(
                        [outerDictEntryParameter],
                        [
                            assignCurrentOuterEntryFromOuterDictEnumerator,
                            addOuterKeyAndValuesToOutput
                        ]),
                    ExitsLoop),
                BreakLabel),

            // Disposes the outer dictionary enumerator after flattening.
            disposeOuterDictEnumerator = Expression.Call(
                outerDictIterVariable,
                TypeUtils.DisposeMethod);

        var fullBlock = Expression.Block(
            [
                outerDictObjEntityVariable, InEntryIterExVariable, outerDictIterVariable, outputVariable
            ], // Declares variables used in the block
            [
                // Initializes dictObjEntity with a new instance of T
                initializeOuterDict,
                // Sets up the enumerator for the input data list.
                SetupInputDataEnumerator,
                // Executes the loop with cleanup
                Expression.TryFinally(
                    Expression.Loop(
                        Expression.IfThenElse(
                            MoveNextOnInEntryEnumerator, // If MoveNext returns true (more rows),
                            // ProcessRowsIfExist
                            Expression.Block(
                                [
                                    CurrentEntryExParameter,
                                    CurrentEntityExVariable,
                                    outerKeyVariable,
                                    innerKeyVariable
                                ], // Local variables for this iteration
                                [
                                    // Sets the current row from the input data enumerator.
                                    AssignCurrentInputRowFromInputEnumerator,
                                    // Creates and assigns the outer key from grouping data.
                                    assignOuterKeyVariableFromGroupingData,
                                    // Sets the inner dictionary key from the row’s "Extend0_Id" value.
                                    assignInnerKeyVariableFromPrimaryKey,
                                    // Initializes a new inner dictionary if the outer key is missing.
                                    initializeInnerDictIfOuterKeyMissing,
                                    // Processes and adds the entity to the inner dictionary if the key is new.
                                    initializeEntityIfInnerKeyMissing,
                                    // Applies additional join processors using the dictionary indexer for related data.
                                    // .. ApplyJoinProcessorsToInnerKeyAccessor
                                    .. Composite.JoinRowProcessors
                                ]),
                            ExitsLoop), // Otherwise, break out of the loop
                        BreakLabel),
                    DisposeInEntryEnumerator),
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
        var lambda = Expression.Lambda<Func<Dictionary<ITuple, List<TReturn>>>>(fullBlock).Compile();

        // Executes the expression tree, returning the result
        return lambda();
    }
}
