namespace KISS.FluentSqlBuilder.Core.Composite;

/// <summary>
///     Implements <see cref="ExpressionVisitor" /> for the <see cref="CompositeQuery" /> type.
///     Traversing and analyzing LINQ expression trees and determining which parts of the expression tree
///     are evaluable at runtime (i.e., whether the expression can be simplified to a value).
/// </summary>
public sealed partial class CompositeQuery : ExpressionVisitor
{
    /// <summary>
    ///     Creates a stack to store expressions as we traverse,
    ///     helping track context for determining if an expression is "Evaluable" (i.e., reducible to a value).
    /// </summary>
    private Stack<Expression> ExpressionStack { get; } = new();

    /// <summary>
    ///     Initializes a dictionary where "Evaluable" means an expression can be computed to a concrete value at runtime;
    ///     keys are expressions, values are true (evaluable) or false (not evaluable).
    /// </summary>
    private Dictionary<Expression, bool> Evaluable { get; } = [];

    /// <summary>
    ///     Depending on the type of expression (e.g., ConstantExpression, MethodCallExpression),
    ///     it marks whether certain expressions are evaluable by setting entries in the Evaluable dictionary.
    /// </summary>
    /// <param name="node">The expression to visit.</param>
    /// <returns>
    ///     The modified expression, if it or any subexpression was modified;
    ///     otherwise, returns the original expression.
    /// </returns>
    public override Expression? Visit(Expression? node)
    {
        // If the node is null, returns null immediately; "Evaluable" doesn’t apply since there’s no expression to evaluate.
        if (node is null)
        {
            return null;
        }

        // Pushes the current node onto the stack; this tracks the expression’s position in the tree,
        // crucial for deciding "Evaluable" status across nested expressions.
        ExpressionStack.Push(node);

        // Switches on the node’s type to determine if it’s "Evaluable"—can it be reduced to a value?—based on its properties and context.
        switch (node)
        {
            // Example: Expression.Call(typeof(Console).GetMethod("WriteLine", [typeof(string)]), Expression.Constant("Hi"))
            //     Evaluable: false (unless part of a valuable BlockExpression)
            //     What: A void-returning method call (e.g., Console.WriteLine("Hi")) that produces no value.
            //     When: Matches when the expression’s Type is void; set to false unless a parent BlockExpression provides a value.
            //     Why: Void expressions don’t yield a result, so they’re not independently evaluable.
            //     GetValue Result: (false, "") (unless in a block with a return value)
            case Expression { Type: Type t } when t == typeof(void):
                // Loops through the stack to assess each expression’s "Evaluable" status, starting from the current void expression.
                foreach (var x in ExpressionStack)
                {
                    // Example: BlockExpression containing the void call above and Expression.Constant(5)
                    //     Evaluable: true (for the block), false (for the void call)
                    //     What: A block with a void call and a final value (5); the void call isn’t evaluable, but the block is.
                    //     When: Stops if a BlockExpression is found that isn’t the current node; blocks can be evaluable based on their last expression.
                    //     Why: Blocks can provide a value despite void sub-expressions, so we preserve their potential evaluability.
                    //     GetValue Result: (true, "5") for the block, (false, "") for the void call
                    if (x is BlockExpression && node != x)
                    {
                        break;
                    }

                    // Sets "Evaluable" to false for the current expression; it’s not evaluable due to its void nature unless a block overrides it.
                    Evaluable[x] = false;
                }

                break;

            // Example: ParameterExpression: Expression.Parameter(typeof(int), "x")
            //     Evaluable: false
            //     What: A placeholder (e.g., x) with no assigned value.
            //     When: Matches ParameterExpression case; set to false for itself and all ancestors in the stack.
            //     Why: Parameters lack a concrete value until runtime binding, making them not evaluable.
            //     GetValue Result: (false, "")
            case ParameterExpression:
            // Example: LoopExpression: Expression.Loop(Expression.Constant(1))
            //     Evaluable: false
            //     What: An infinite loop (e.g., while(true) { 1; }) that may never produce a final value.
            //     When: Matches LoopExpression case; set to false for itself and ancestors.
            //     Why: Loops might not terminate, so they’re not evaluable; this propagates to containing expressions.
            //     GetValue Result: (false, "")
            case LoopExpression:
            case { NodeType: ExpressionType.Extension, CanReduce: false }:
                // Loops through the stack to update "Evaluable" for all expressions up to the root.
                foreach (var x in ExpressionStack)
                {
                    // Sets "Evaluable" to false; these types (Parameters, Loops, non-reducible Extensions) can’t be simplified to a value.
                    Evaluable[x] = false;
                }

                break;

            case DefaultExpression:
            // Example: ConstantExpression: Expression.Constant(42)
            //     Evaluable: true
            //     What: A fixed value (e.g., 42) that can be directly evaluated.
            //     When: Matches ConstantExpression case; set to true unless it’s a Lambda or Block.
            //     Why: Constants are standalone values with no dependencies, making them evaluable.
            //     GetValue Result: (true, "42")
            case ConstantExpression:
            case MethodCallExpression { Arguments.Count: 0, Object: null }:
            case MemberExpression { Expression: null }:
            case NewExpression { Arguments.Count: 0, Members.Count: 0 }:
            case DebugInfoExpression:
            case GotoExpression:
                // Loops through the stack to determine "Evaluable" for each expression, starting with the current node.
                foreach (var x in ExpressionStack)
                {
                    if (Evaluable.ContainsKey(x))
                    {
                        // Stops if already assessed to avoid overwriting "Evaluable."
                        break;
                    }

                    // Example: LambdaExpression: Expression.Lambda(Expression.Constant(10))
                    //     Evaluable: false
                    //     What: A lambda wrapping a constant (10); the lambda itself isn’t a value.
                    //     When: Set to false if it’s a LambdaExpression; true otherwise (e.g., for the ConstantExpression alone).
                    //     Why: Lambdas depend on their body, so they’re not evaluable themselves; standalone expressions are.
                    //     GetValue Result: (false, "") for lambda, (true, "10") for its body
                    // BlockExpression's value is the same as the last expression in the block
                    // in either case we will have gotten the value for the underlying expression; there's no need to do so again
                    // Sets "Evaluable" to true for simple expressions, false for Lambdas/Blocks.
                    Evaluable[x] = x is not (LambdaExpression or BlockExpression);
                }

                break;
        }

        // Example: Extension (non-reducible): CustomExpression { CanReduce: false }
        //     Evaluable: false (assumed by logic flow)
        //     What: A custom expression type that can’t be simplified further.
        //     When: Matches non-reducible Extension case; returned unchanged without altering "Evaluable."
        //     Why: Non-reducible extensions can’t be evaluated to a value in this context.
        //     GetValue Result: (false, "") (unless Visit later determines otherwise)
        var ret = node is { NodeType: ExpressionType.Extension, CanReduce: false }
            ? node // Returns unchanged; not inherently "Evaluable" beyond its form.
            : base.Visit(node); // Visits sub-expressions to further assess "Evaluable" status.

        // Removes the node from the stack; "Evaluable" is set, and tracking is done.
        ExpressionStack.Pop();

        // Returns the final expression, reflecting "Evaluable" updates.
        return ret;
    }

    /// <summary>
    ///     Determines if a specific expression can be evaluated.
    /// </summary>
    /// <param name="node">The expression to visit.</param>
    /// <returns>
    ///     A tuple of a boolean (Evaluated) indicating whether the expression was evaluable,
    ///     and the evaluated result (Value) as a FormatString.
    /// </returns>
    public (bool Evaluated, FormattableString Value) GetValue(Expression node)
    {
        // Example: Unknown Expression: Expression.Constant(100) (not yet visited)
        //     Evaluable: unknown (false by default until visited)
        //     What: A constant (100) not yet analyzed for evaluability.
        //     When: First check fails if not in Evaluable; triggers Visit to determine status.
        //     Why: We need to traverse it to confirm it’s evaluable (will be true after Visit).
        //     GetValue Result: Determined after Visit (true, "100")
        if (!Evaluable.TryGetValue(
                node,
                out var canEvaluate)) // Checks if "Evaluable" is known; if not, it’s unassessed.
        {
            // Analyzes the node to set its "Evaluable" status in the dictionary.
            Visit(node);
            // Retrieves the updated "Evaluable" status post-visit.
            Evaluable.TryGetValue(node, out canEvaluate);
        }

        // Example: ParameterExpression: Expression.Parameter(typeof(int), "x")
        //     Evaluable: false
        //     What: A parameter (x) that can’t be evaluated to a value.
        //     When: After Visit confirms it’s not evaluable; returns false.
        //     Why: No concrete value exists, so evaluation fails.
        //     GetValue Result: (false, "")
        if (!canEvaluate)
        {
            // Returns false and empty string if not "Evaluable."
            return (false, $"");
        }

        // Example: ConstantExpression: Expression.Constant(42)
        //     Evaluable: true
        //     What: A constant (42) that can be evaluated.
        //     When: After Visit confirms it’s evaluable; proceeds to lambda creation.
        //     Why: It’s a fixed value, so it can be computed.
        //     GetValue Result: (true, "42")
        var lambdaExpression = Expression.Lambda(node); // Wraps the evaluable node in a lambda for execution.

        // Compiles and runs the lambda, returning the value since it’s "Evaluable."
        return (true, $"{lambdaExpression.Compile().DynamicInvoke()}");
    }
}
