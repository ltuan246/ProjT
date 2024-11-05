namespace KISS.QueryBuilder.Core;

/// <summary>
///     Implements <see cref="ExpressionVisitor" /> for the <see cref="FluentSqlBuilder{TRecordset}" /> type.
///     Traversing and analyzing LINQ expression trees and determining which parts of the expression tree
///     are evaluable at runtime (i.e., whether the expression can be simplified to a value).
/// </summary>
internal sealed partial class QueryVisitor : ExpressionVisitor
{
    /// <summary>
    ///     A stack that holds expressions during the traversal process. This is used to track where we are in the tree.
    /// </summary>
    private Stack<Expression> ExpressionStack { get; } = new();

    /// <summary>
    ///     Keeps track of which expressions can be evaluated to a value.
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
        if (node is null)
        {
            return null;
        }

        // pushes the current expression onto the ExpressionStack and performs different actions based on the type of expression (node).
        ExpressionStack.Push(node);

        switch (node)
        {
            // The only way to get a void-returning expression within an expression tree is as part of a BlockExpression
            // other expression types cannot contain a void-returning expression
            case { Type: var type } when type == typeof(void):
                foreach (var x in ExpressionStack)
                {
                    // BlockExpressions may have a value even though one of the statements is a void-returning expression
                    if (x is BlockExpression && node != x)
                    {
                        break;
                    }

                    Evaluable[x] = false;
                }

                break;

            // Nodes which contain the following expression types cannot be evaluated
            // ParameterExpression because it has no value
            // LoopExpression because it might be a never-ending loop
            case ParameterExpression:
            case LoopExpression:
            case { NodeType: ExpressionType.Extension, CanReduce: false }:
                foreach (var x in ExpressionStack)
                {
                    Evaluable[x] = false;
                }

                break;

            case DefaultExpression:
            case ConstantExpression:
            case MethodCallExpression { Arguments.Count: 0, Object: null }:
            case MemberExpression { Expression: null }:
            case NewExpression { Arguments.Count: 0, Members.Count: 0 }:
            case DebugInfoExpression:
            case GotoExpression:
                foreach (var x in ExpressionStack)
                {
                    if (Evaluable.ContainsKey(x))
                    {
                        break;
                    }

                    // LambdaExpression's value is the same as LambdaExpression.Body
                    // BlockExpression's value is the same as the last expression in the block
                    // in either case we will have gotten the value for the underlying expression; there's no need to do so again
                    Evaluable[x] = x is not LambdaExpression and not BlockExpression;
                }

                break;
        }

        var ret = node switch
        {
            { NodeType: ExpressionType.Extension, CanReduce: false } => node,
            _ => base.Visit(node)
        };

        // After processing, it pops the expression from the stack.
        ExpressionStack.Pop();

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
    private (bool Evaluated, FormattableString Value) GetValue(Expression node)
    {
        if (!Evaluable.TryGetValue(node, out var canEvaluate))
        {
            Visit(node);
            Evaluable.TryGetValue(node, out canEvaluate);
        }

        /*
         Understanding canReduce/canEvaluate:
         - CanReduce = true: The expression can be simplified or transformed to another expression that is semantically equivalent but simpler or more direct.
            If an expression involves a function call or any computation that requires execution to determine the final result, it as reducible.
         - CanReduce = false: The expression is already in its simplest form, or simplification would not yield a valid expression.
            If an expression is a basic data type and does not require any further computation, it as non-reducible.
         */

        if (!canEvaluate)
        {
            return (false, $"");
        }

        var lambdaExpression = Expression.Lambda(node);
        return (canEvaluate, $"{lambdaExpression.Compile().DynamicInvoke()}");
    }
}
