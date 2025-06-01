namespace KISS.FluentSqlBuilder.Decorators.JoinDecorators;

/// <summary>
///     Provides expression variables and logic for building and executing join operations
///     in the JoinDecorator. Handles the construction of expression trees for joining entities,
///     managing dictionary-based entity collections, and processing input rows for SQL JOIN queries.
/// </summary>
public sealed partial record JoinDecorator
{
    /// <summary>
    ///     Expression variable representing the output dictionary for joined entities.
    ///     Maps object keys to output entity instances.
    /// </summary>
    private ParameterExpression OutDictEntityTypeExVariable { get; }
        = Expression.Variable(
            TypeUtils.DictionaryType.MakeGenericType([TypeUtils.ObjType, Inner.OutEntityType]),
            "OutDictEntityTypeExVariable");

    /// <summary>
    ///     Expression variable representing the key used for the output dictionary.
    /// </summary>
    private ParameterExpression OutDictKeyExVariable { get; } =
        Expression.Variable(TypeUtils.ObjType, "OutDictKeyExVariable");

    /// <summary>
    ///     Constant expression representing the primary key field name ("Extend0_Id").
    /// </summary>
    private ConstantExpression PrimaryKey { get; } = Expression.Constant("Extend0_Id");

    /// <summary>
    ///     Expression that initializes the output dictionary variable.
    /// </summary>
    private BinaryExpression InitializeDictVariable
        => TypeUtils.InitializeTargetValue(OutDictEntityTypeExVariable);

    /// <summary>
    ///     Expression that initializes the dictionary key accessor and adds new entities if the key is missing.
    ///     Handles extraction and conversion of the primary key, entity creation, and insertion into the dictionary.
    /// </summary>
    private BinaryExpression InitializeDictKeyAccessor
        => TypeUtils.InitializeTargetValue(
            IndexerExVariable,
            Expression.Block(
            [
                // Extracts and converts the "Extend0_Id" key from the row to an object.
                TypeUtils.InitializeTargetValue(
                    OutDictKeyExVariable,
                    TypeUtils.ChangeType(
                        Expression.Property(CurrentEntryExVariable, "Item", PrimaryKey),
                        TypeUtils.ObjType)),

                // If the key is not present in the dictionary, create and add a new entity.
                Expression.IfThen(
                    Expression.Not(TypeUtils.IsDictContainsKey(OutDictEntityTypeExVariable, OutDictKeyExVariable)),
                    Expression.Block(
                        [],
                        // Initialize the entity from the current row.
                        TypeUtils.InitializeTargetValue(
                            CurrentEntityExVariable,
                            TypeUtils.CreateIterRowBindings(
                                CurrentEntryExVariable,
                                InEntityType,
                                CurrentEntityExVariable.Type,
                                GetAliasMapping(InEntityType))),
                        // Add the new entity to the dictionary.
                        TypeUtils.CallMethod(
                            OutDictEntityTypeExVariable,
                            "Add",
                            OutDictKeyExVariable,
                            CurrentEntityExVariable))),

                // Access the entity in the dictionary by key.
                Expression.MakeIndex(
                    OutDictEntityTypeExVariable,
                    OutDictEntityTypeExVariable.Type.GetProperty("Item")!,
                    [OutDictKeyExVariable])
            ]));

    /// <inheritdoc />
    public override BlockExpression Block
    {
        get
        {
            var breakLabel = Expression.Label();
            var exitsLoop = Expression.Break(breakLabel);

            var block = Expression.Block(
                [
                    // Declare variables used in the block.
                    InEntriesExVariable,
                    OutEntitiesExVariable,
                    OutDictEntityTypeExVariable,
                    IndexerExVariable
                ],
                [
                    // Initialize the output list variable.
                    TypeUtils.InitializeTargetValue(OutEntitiesExVariable),

                    // Set up the enumerator for the input data.
                    TypeUtils.InitializeTargetValue(
                        InEntriesExVariable,
                        Expression.Call(InEntriesExParameter, TypeUtils.GetEnumeratorForIEnumDictStrObj)),

                    // Initialize the output dictionary variable.
                    InitializeDictVariable,

                    // Main processing loop with cleanup.
                    Expression.TryFinally(
                        Expression.Loop(
                            Expression.IfThenElse(
                                Expression.Call(
                                    InEntriesExVariable,
                                    TypeUtils.IterMoveNextMethod), // If more rows exist,
                                Expression.Block(
                                    [
                                        CurrentEntryExVariable,
                                        CurrentEntityExVariable,
                                        OutDictKeyExVariable
                                    ],
                                    [
                                        // Assign the current row from the enumerator.
                                        TypeUtils.InitializeTargetValue(
                                            CurrentEntryExVariable,
                                            Expression.Property(InEntriesExVariable, "Current")),

                                        // Initialize the dictionary key accessor and add entity if needed.
                                        InitializeDictKeyAccessor,

                                        // Apply additional join processors using the dictionary indexer.
                                        .. JoinRows
                                    ]),
                                exitsLoop), // Otherwise, exit the loop
                            breakLabel),
                        Expression.Call(InEntriesExVariable, TypeUtils.DisposeMethod)),

                    // Add all dictionary values to the output list.
                    TypeUtils.CallMethod(
                        OutEntitiesExVariable,
                        "AddRange",
                        Expression.Property(OutDictEntityTypeExVariable, "Values")),

                    // Return the populated output list.
                    OutEntitiesExVariable
                ]);

            return block;
        }
    }
}
