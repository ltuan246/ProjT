namespace KISS.FluentSqlBuilder.Decorators.JoinDecorators;

/// <summary>
///     Contains expression variables and logic for constructing and executing join operations
///     within the JoinDecorator. Responsible for building expression trees that perform entity joins,
///     manage dictionary-based collections, and process input rows for SQL JOIN queries.
/// </summary>
public sealed partial record JoinDecorator
{
    /// <summary>
    ///     Expression variable for the output dictionary that maps object keys to joined entity instances.
    /// </summary>
    private ParameterExpression OutDictEntityTypeExVariable { get; }
        = Expression.Variable(
            TypeUtils.DictionaryType.MakeGenericType([TypeUtils.TupleType, Inner.OutEntityType]),
            "OutDictEntityTypeExVariable");

    /// <summary>
    ///     Expression variable representing the key used for the output dictionary.
    /// </summary>
    private ParameterExpression OutDictKeyExVariable { get; } =
        Expression.Variable(TypeUtils.TupleType, "OutDictKeyExVariable");

    /// <inheritdoc />
    public override BlockExpression Block
    {
        get
        {
            var breakLabel = Expression.Label();
            var exitsLoop = Expression.Break(breakLabel);

            var primaryKeys = InEntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.GetCustomAttributes(typeof(KeyBuilderAttribute), true).Length != 0)
                .ToArray();

            var primaryKeyTypes = primaryKeys.Select(pI => pI.PropertyType).ToArray();

            // Dynamically create the ValueTuple type for the join key.
            var outerKeyConstructor =
                Type.GetType($"{TypeUtils.ValueTupleType.FullName}`{primaryKeys.Length}")!
                    .MakeGenericType(primaryKeyTypes)
                    .GetConstructor(primaryKeyTypes)!;

            // Build constructor arguments for the join key from the current input row.
            var outerKeyArguments = primaryKeys
                .Select(k =>
                    TypeUtils.ChangeType(
                        Expression.Property(CurrentEntryExVariable, "Item", Expression.Constant($"Extend0_{k.Name}")),
                        k.PropertyType))
                .ToArray();

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

                    // Initialize the enumerator for the input data.
                    TypeUtils.InitializeTargetValue(
                        InEntriesExVariable,
                        Expression.Call(InEntriesExParameter, TypeUtils.GetEnumeratorForIEnumDictStrObj)),

                    // Initialize the output dictionary variable.
                    TypeUtils.InitializeTargetValue(OutDictEntityTypeExVariable),

                    // Main processing loop with cleanup.
                    Expression.TryFinally(
                        Expression.Loop(
                            Expression.IfThenElse(
                                Expression.Call(
                                    InEntriesExVariable,
                                    TypeUtils.IterMoveNextMethod), // Continue if more rows exist
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
                                        TypeUtils.InitializeTargetValue(
                                            IndexerExVariable,
                                            Expression.Block(
                                            [
                                                // Extract and convert the primary key from the row to an object.
                                                TypeUtils.InitializeTargetValue(
                                                    OutDictKeyExVariable,
                                                    Expression.Convert(Expression.New(outerKeyConstructor, outerKeyArguments), TypeUtils.TupleType)),

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
                                            ])),

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
