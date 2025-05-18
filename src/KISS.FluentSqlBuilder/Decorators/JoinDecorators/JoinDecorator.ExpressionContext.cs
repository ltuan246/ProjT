namespace KISS.FluentSqlBuilder.Decorators.JoinDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public sealed partial record JoinDecorator
{
    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    public ParameterExpression OutDictEntityTypeExVariable { get; }
        = Expression.Variable(TypeUtils.DictionaryType.MakeGenericType([TypeUtils.ObjType, Inner.OutEntityType]), "OutDictEntityTypeExVariable");

    /// <summary>
    /// OutDictEntityType.
    /// </summary>
    public ParameterExpression OutDictKeyExVariable { get; } = Expression.Variable(TypeUtils.ObjType, "OutDictKeyExVariable");

    /// <summary>
    /// OutDictKeyAccessorExVariable.
    /// </summary>
    public ParameterExpression OutDictKeyAccessorExVariable { get; } = Expression.Variable(Inner.OutEntityType, "OutDictKeyAccessorExVariable");

    /// <summary>
    ///     A function that define how to process each row.
    /// </summary>
    public List<Expression> JoinRowProcessors { get; } = [];

    /// <inheritdoc />
    public override BlockExpression Block
    {
        get
        {
            var breakLabel = Expression.Label();
            var exitsLoop = Expression.Break(breakLabel);
            var primaryKey = Expression.Constant("Extend0_Id");

            var block = Expression.Block(
                [
                    // Declares variables used in the block
                    InEntriesExVariable,
                    OutEntitiesExVariable,
                ],
                [
                    // Initialize outputList with a new list
                    // InitializeOutputVariable,
                    TypeUtils.InitializeTargetValue(OutEntitiesExVariable),

                    // Sets up the enumerator for inputData
                    // SetupInputDataEnumerator,
                    Expression.Assign(
                        InEntriesExVariable,
                        Expression.Call(InEntriesExParameter, TypeUtils.GetEnumeratorForIEnumDictStrObj)),

                    Expression.Block(
                        [
                            OutDictEntityTypeExVariable,
                            OutDictKeyAccessorExVariable,
                        ],
                        [
                            // InitializeDictVariable: Initializes dictObjEntity with a new instance of T
                            TypeUtils.InitializeTargetValue(OutDictEntityTypeExVariable),

                            // Executes the loop with cleanup
                            Expression.TryFinally(
                                Expression.Loop(
                                    Expression.IfThenElse(
                                        Expression.Call(InEntriesExVariable, TypeUtils.IterMoveNextMethod), // If MoveNext returns true (more rows),
                                        Expression.Block(
                                            [
                                                CurrentEntryExVariable,
                                                CurrentEntityExVariable,
                                            ],
                                            [
                                                // Assigns the current row from the enumerator to iterationRowParameter.
                                                // AssignCurrentInputRowFromInputEnumerator,
                                                Expression.Assign(
                                                    CurrentEntryExVariable,
                                                    Expression.Property(InEntriesExVariable, "Current")),

                                                Expression.Block(
                                                    [
                                                        OutDictKeyExVariable!
                                                    ],
                                                    [
                                                        // Extracts and converts the "Extend0_Id" key from the row to a string.
                                                        Expression.Assign(
                                                            OutDictKeyExVariable!,
                                                            TypeUtils.ChangeType(
                                                                Expression.Property(CurrentEntryExVariable, "Item", primaryKey),
                                                                TypeUtils.ObjType)),

                                                        // InitializeEntityIfKeyMissing: Processes the row if its key isn’t already in the dictionary.
                                                        Expression.IfThen(
                                                            Expression.Not(TypeUtils.IsDictContainsKey(OutDictEntityTypeExVariable, OutDictKeyExVariable!)),
                                                            Expression.Block(
                                                                [], // Ensures variables is scoped for this operation
                                                                // Applies the row processor to construct or modify the entity.
                                                                TypeUtils.InitializeTargetValue(
                                                                    CurrentEntityExVariable,
                                                                    TypeUtils.CreateIterRowBindings(
                                                                        CurrentEntryExVariable,
                                                                        InEntityType!,
                                                                        CurrentEntityExVariable.Type,
                                                                        GetAliasMapping(InEntityType!))),
                                                                // Adds the processed entity to the dictionary with its key.
                                                                TypeUtils.CallMethod(
                                                                    OutDictEntityTypeExVariable,
                                                                    "Add",
                                                                    OutDictKeyExVariable!,
                                                                    CurrentEntityExVariable))),

                                                        Expression.Assign(
                                                            OutDictKeyAccessorExVariable!,
                                                            Expression.MakeIndex(
                                                                OutDictEntityTypeExVariable!,
                                                                OutDictEntityTypeExVariable!.Type.GetProperty("Item")!,
                                                                [OutDictKeyExVariable])),

                                                        // Applies additional join processors using the dictionary indexer for related data.
                                                        // .. ApplyJoinProcessorsToInnerKeyAccessor
                                                        .. JoinRowProcessors
                                                    ]),
                                            ]),
                                        exitsLoop), // Otherwise, break out of the loop
                                    breakLabel),
                                Expression.Call(InEntriesExVariable, TypeUtils.DisposeMethod)),

                            // Convert dictionary values to list
                            TypeUtils.CallMethod(
                                OutEntitiesExVariable,
                                "AddRange",
                                Expression.Property(OutDictEntityTypeExVariable, "Values")),
                        ]),

                    // Return the populated list
                    OutEntitiesExVariable
                ]);

            return block;
        }
    }
}