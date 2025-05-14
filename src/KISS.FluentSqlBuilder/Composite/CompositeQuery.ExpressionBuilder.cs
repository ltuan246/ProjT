// namespace KISS.FluentSqlBuilder.Composite;

// /// <summary>
// ///     A context for storing reusable instances used in expression tree construction.
// /// </summary>
// public abstract partial record CompositeQuery
// {
//     /// <inheritdoc />
//     public (BlockExpression Block, ParameterExpression InputDataExParameter) CreateExpressionBlock()
//     {
//         var inputDataExParameter = Expression.Parameter(TypeUtils.DapperRowCollectionType, "inputDataExParameter");
//         var breakLabel = Expression.Label();
//         var exitsLoop = Expression.Break(breakLabel);
//         var block = Expression.Block(
//             [
//                 InEntryIterExVariable,
//                 OutEntitiesExVariable
//             ], // Declares variables used in the block
//             [
//                 // Initializes outputCollection with a new instance of T
//                 // InitializeOutputVariable,
//                 TypeUtils.InitializeTargetValue(OutEntitiesExVariable),
//                 // Sets up the enumerator for inputData
//                 // SetupInputDataEnumerator,
//                 Expression.Assign(
//                     InEntryIterExVariable,
//                     Expression.Call(inputDataExParameter, TypeUtils.GetEnumeratorForIEnumDictStrObj)),
//                 // Executes the loop with cleanup
//                 Expression.TryFinally(
//                     Expression.Loop(
//                         Expression.IfThenElse(
//                             Expression.Call(InEntryIterExVariable, TypeUtils.IterMoveNextMethod), // If MoveNext returns true (more rows),
//                             Expression.Block(
//                                 [
//                                     CurrentEntryExParameter,
//                                     CurrentEntityExVariable
//                                 ],
//                                 [
//                                     // Execute the loop body with the current row
//                                     // AssignCurrentInputRowFromInputEnumerator,
//                                     Expression.Assign(
//                                         CurrentEntryExParameter,
//                                         Expression.Property(InEntryIterExVariable, "Current")),
//                                     // InitializeEntityIfKeyMissing
//                                     TypeUtils.InitializeTargetValue(
//                                         CurrentEntityExVariable,
//                                         TypeUtils.CreateIterRowBindings(
//                                             CurrentEntryExParameter,
//                                             InEntityType,
//                                             CurrentEntityExVariable.Type,
//                                             GetAliasMapping(InEntityType))),
//                                     // Adds the processed entity to the dictionary with its key.
//                                     TypeUtils.CallMethod(
//                                         OutEntitiesExVariable, // Calls the Add method on the output list.
//                                         "Add",
//                                         CurrentEntityExVariable)
//                                 ]),
//                             exitsLoop), // Otherwise, break out of the loop
//                         breakLabel),
//                     Expression.Call(InEntryIterExVariable, TypeUtils.DisposeMethod)),
//                 // Returns the populated collection
//                 OutEntitiesExVariable
//             ]);
//         return (block, inputDataExParameter);
//     }
// }