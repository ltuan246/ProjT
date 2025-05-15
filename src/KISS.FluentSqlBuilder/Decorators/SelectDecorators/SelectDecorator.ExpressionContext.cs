namespace KISS.FluentSqlBuilder.Decorators.SelectDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
/// <typeparam name="TIn">The type representing the database record set.</typeparam>
/// <typeparam name="TOut">The combined type to return.</typeparam>
public sealed partial record SelectDecorator<TIn, TOut>
{
    /// <inheritdoc />
    public ParameterExpression InEntriesExParameter { get; init; }

    /// <inheritdoc />
    public ParameterExpression InEntriesExVariable { get; init; }

    /// <inheritdoc />
    public ParameterExpression OutEntitiesExVariable { get; init; }

    /// <inheritdoc />
    public ParameterExpression CurrentEntryExVariable { get; init; }

    /// <inheritdoc />
    public ParameterExpression CurrentEntityExVariable { get; init; }

    /// <inheritdoc />
    public BlockExpression Block
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
                    // Initializes outputCollection with a new instance of T
                    // InitializeOutputVariable,
                    TypeUtils.InitializeTargetValue(OutEntitiesExVariable),
                    // Sets up the enumerator for inputData
                    // SetupInputDataEnumerator,
                    Expression.Assign(
                        InEntriesExVariable,
                        Expression.Call(InEntriesExParameter, TypeUtils.GetEnumeratorForIEnumDictStrObj)),
                    // Executes the loop with cleanup
                    Expression.TryFinally(
                        Expression.Loop(
                            Expression.IfThenElse(
                                Expression.Call(InEntriesExVariable, TypeUtils.IterMoveNextMethod), // If MoveNext returns true (more rows),
                                Expression.Block(
                                    [
                                        CurrentEntryExVariable,
                                        CurrentEntityExVariable
                                    ],
                                    [
                                        // Execute the loop body with the current row
                                        // AssignCurrentInputRowFromInputEnumerator,
                                        Expression.Assign(
                                            CurrentEntryExVariable,
                                            Expression.Property(InEntriesExVariable, "Current")),
                                        // InitializeEntityIfKeyMissing
                                        TypeUtils.InitializeTargetValue(
                                            CurrentEntityExVariable,
                                            TypeUtils.CreateIterRowBindings(
                                                CurrentEntryExVariable,
                                                InEntityType,
                                                CurrentEntityExVariable.Type,
                                                GetAliasMapping(InEntityType))),
                                        // Adds the processed entity to the dictionary with its key.
                                        TypeUtils.CallMethod(
                                            OutEntitiesExVariable, // Calls the Add method on the output list.
                                            "Add",
                                            CurrentEntityExVariable)
                                    ]),
                                exitsLoop), // Otherwise, break out of the loop
                            breakLabel),
                        Expression.Call(InEntriesExVariable, TypeUtils.DisposeMethod)),
                    // Returns the populated collection
                    OutEntitiesExVariable
                ]);

            return block;
        }
    }
}