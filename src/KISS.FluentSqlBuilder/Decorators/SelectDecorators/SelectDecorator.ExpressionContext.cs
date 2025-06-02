namespace KISS.FluentSqlBuilder.Decorators.SelectDecorators;

/// <summary>
///     Provides the expression tree logic for projecting and materializing entities in the SelectDecorator.
///     This class builds the LINQ expression block that iterates through input rows, creates entity instances,
///     and collects them into the output collection for SQL SELECT operations.
/// </summary>
public sealed partial record SelectDecorator
{
    /// <summary>
    ///     Gets the expression block that processes input rows, creates entities, and collects them into the output list.
    ///     Handles enumerator setup, entity instantiation, and resource cleanup for SELECT queries.
    /// </summary>
    public override BlockExpression Block
    {
        get
        {
            var breakLabel = Expression.Label();
            var exitsLoop = Expression.Break(breakLabel);

            var block = Expression.Block(
                [
                    InEntriesExVariable,
                    OutEntitiesExVariable
                ], // Declares variables used in the block
                [
                    // Initialize the output collection variable.
                    TypeUtils.InitializeTargetValue(OutEntitiesExVariable),

                    // Set up the enumerator for the input data collection.
                    TypeUtils.InitializeTargetValue(
                        InEntriesExVariable,
                        Expression.Call(InEntriesExParameter, TypeUtils.GetEnumeratorForIEnumDictStrObj)),

                    // Main processing loop with cleanup.
                    Expression.TryFinally(
                        Expression.Loop(
                            Expression.IfThenElse(
                                Expression.Call(
                                    InEntriesExVariable,
                                    TypeUtils.IterMoveNextMethod), // If MoveNext returns true (more rows),
                                Expression.Block(
                                    [
                                        CurrentEntryExVariable,
                                        CurrentEntityExVariable
                                    ],
                                    [
                                        // Assign the current row from the input enumerator.
                                        TypeUtils.InitializeTargetValue(
                                            CurrentEntryExVariable,
                                            Expression.Property(InEntriesExVariable, "Current")),

                                        // Initialize the entity from the current row.
                                        TypeUtils.InitializeTargetValue(
                                            CurrentEntityExVariable,
                                            TypeUtils.CreateIterRowBindings(
                                                CurrentEntryExVariable,
                                                InEntityType,
                                                CurrentEntityExVariable.Type,
                                                GetAliasMapping(InEntityType))),

                                        // Add the processed entity to the output collection.
                                        TypeUtils.CallMethod(
                                            OutEntitiesExVariable,
                                            "Add",
                                            CurrentEntityExVariable)
                                    ]),
                                exitsLoop), // Otherwise, break out of the loop
                            breakLabel),
                        // Dispose the enumerator after processing.
                        Expression.Call(InEntriesExVariable, TypeUtils.DisposeMethod)),

                    // Return the populated output collection.
                    OutEntitiesExVariable
                ]);

            return block;
        }
    }
}
