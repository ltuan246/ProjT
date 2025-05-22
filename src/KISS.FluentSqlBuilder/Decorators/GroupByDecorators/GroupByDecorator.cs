namespace KISS.FluentSqlBuilder.Decorators.GroupByDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public sealed partial record GroupByDecorator : QueryDecorator
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GroupByDecorator"/> class.
    /// </summary>
    /// <param name="inner">inner.</param>
    public GroupByDecorator(IComposite inner)
        : base(inner)
    {
        OutDictEntitiesType = TypeUtils.DictionaryType.MakeGenericType([OuterKeyType, Inner.OutEntitiesType]);

        InnerDictObjEntityType = TypeUtils.DictionaryType.MakeGenericType([TypeUtils.ObjType, Inner.OutEntityType]);
        OuterDictTupleEntityCollectionType = TypeUtils.DictionaryType.MakeGenericType([OuterKeyType, InnerDictObjEntityType]);

        OuterIterType = TypeUtils.DictionaryType.MakeGenericType([TypeUtils.ObjType, Inner.OutEntityType]).GetNestedType("Enumerator")!.GetType();
        OuterEntryType = typeof(KeyValuePair<,>).MakeGenericType([TypeUtils.ObjType, Inner.OutEntityType]);

        OutputVariable = Expression.Variable(OutDictEntitiesType, "OutputVariable");
        OuterDictObjEntityVariable = Expression.Variable(OuterDictTupleEntityCollectionType, "OuterDictTupleEntityCollectionType");
        OuterKeyVariable = Expression.Variable(OuterKeyType, "OuterKeyVariable");
        InnerKeyVariable = Expression.Variable(InnerKeyType, "InnerKeyVariable");
        OuterDictIterVariable = Expression.Variable(OuterIterType, "OuterDictIterVariable");
        OuterDictEntryParameter = Expression.Parameter(OuterEntryType, "OuterDictEntryParameter");
    }

    /// <summary>
    /// OutputVariable.
    /// </summary>
    public ParameterExpression OutputVariable { get; init; }

    /// <summary>
    /// OutputVariable.
    /// </summary>
    public ParameterExpression OuterDictObjEntityVariable { get; init; }

    /// <summary>
    /// OutputVariable.
    /// </summary>
    public ParameterExpression OuterKeyVariable { get; init; }

    /// <summary>
    /// OutputVariable.
    /// </summary>
    public ParameterExpression InnerKeyVariable { get; init; }

    /// <summary>
    /// OutputVariable.
    /// </summary>
    public ParameterExpression OuterDictIterVariable { get; init; }

    /// <summary>
    /// OutputVariable.
    /// </summary>
    public ParameterExpression OuterDictEntryParameter { get; init; }

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
                        TypeUtils.CallMethod(OuterDictObjEntityVariable, "Add", OuterKeyVariable, Expression.New(InnerDictObjEntityType)))),
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
                    OutputVariable
                ], // Declares variables used in the block
                [
                    // Initializes dictObjEntity with a new instance of T
                    initializeOuterDict,
                    // Sets up the enumerator for the input data list.
                    // SetupInputDataEnumerator,
                    Expression.Assign(
                        InEntriesExVariable,
                        Expression.Call(InEntriesExParameter, TypeUtils.GetEnumeratorForIEnumDictStrObj)),
                    // Executes the loop with cleanup
                    Expression.TryFinally(
                        Expression.Loop(
                            Expression.IfThenElse(
                                Expression.Call(InEntriesExVariable, TypeUtils.IterMoveNextMethod), // If MoveNext returns true (more rows),
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
                                        Expression.Assign(
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

    /// <inheritdoc />
    public override string Sql
    {
        get
        {
            Append("WITH CommonTableExpression AS (");
            AppendLine(Inner.Sql);
            AppendLine(")");

            new EnumeratorProcessor<string>(SqlStatements[SqlStatement.GroupBy])
                .AccessFirst(_ =>
                {
                    StringBuilder outerSelectBuilder = new(),
                        innerSelectBuilder = new(),
                        groupByFilteringBuilder = new(),
                        onClauseBuilder = new();

                    new EnumeratorProcessor<KeyValuePair<string, Type>>(AggregationKeys)
                        .AccessFirst(kv =>
                        {
                            outerSelectBuilder.Append($"GP.{kv.Key}");
                        })
                        .AccessRemaining(kv =>
                        {
                            outerSelectBuilder.AppendLine($", GP.{kv.Key}");
                        })
                        .AccessLast(() => outerSelectBuilder.Append(','))
                        .Execute();

                    new EnumeratorProcessor<KeyValuePair<string, Type>>(GroupingKeys)
                        .AccessFirst(kv =>
                        {
                            outerSelectBuilder.Append($"GP.{kv.Key}");
                            innerSelectBuilder.Append($"{kv.Key}");
                            groupByFilteringBuilder.Append($"GROUP BY {kv.Key}");
                            onClauseBuilder.Append($"CTE.{kv.Key} = GP.{kv.Key}");
                        })
                        .AccessRemaining(kv =>
                        {
                            outerSelectBuilder.AppendLine($", GP.{kv.Key}");
                            innerSelectBuilder.AppendLine($", {kv.Key}");
                            groupByFilteringBuilder.AppendLine($", {kv.Key}");
                            onClauseBuilder.AppendLine(
                                $"AND CTE.{kv.Key} = GP.{kv.Key}");
                        })
                        .AccessLast(() => outerSelectBuilder.Append(','))
                        .Execute();

                    outerSelectBuilder.Append("CTE.*");

                    new EnumeratorProcessor<string>(SqlStatements[SqlStatement.SelectAggregate])
                        .AccessFirst(fs =>
                        {
                            if (innerSelectBuilder.Length > 0)
                            {
                                innerSelectBuilder.Append(',');
                            }

                            innerSelectBuilder.Append($" {fs}");
                        })
                        .AccessRemaining(fs =>
                        {
                            innerSelectBuilder.AppendLine($", {fs}");
                        })
                        .Execute();

                    new EnumeratorProcessor<string>(SqlStatements[SqlStatement.Having])
                        .AccessFirst(fs =>
                        {
                            groupByFilteringBuilder.Append($" HAVING {fs}");
                        })
                        .AccessRemaining(fs =>
                        {
                            groupByFilteringBuilder.AppendLine($"AND {fs}");
                        })
                        .Execute();

                    Append($@"
                        SELECT
                            {outerSelectBuilder}
                        FROM CommonTableExpression CTE
                        JOIN (
                            SELECT
                                {innerSelectBuilder}
                            FROM CommonTableExpression
                                {groupByFilteringBuilder}
                        ) GP
                        ON {onClauseBuilder};
                    ");

                    AppendLine();
                })
                .Execute();

            return SqlBuilder.ToString();
        }
    }
}