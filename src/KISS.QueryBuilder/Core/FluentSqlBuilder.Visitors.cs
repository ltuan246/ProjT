namespace KISS.QueryBuilder.Core;

/// <summary>
///     Implements <see cref="ExpressionVisitor" /> for the <see cref="FluentSqlBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of results to return.</typeparam>
public sealed partial class FluentSqlBuilder<TEntity> : ExpressionVisitor
{
    private Stack<Expression> ExpressionStack { get; } = new();

    private Dictionary<Expression, bool> Evaluable { get; } = [];

    /// <inheritdoc />
    public override Expression? Visit(Expression? node)
    {
        if (node is null) { return null; }

        ExpressionStack.Push(node);

        switch (node)
        {
            // The only way to get a void-returning expression within an expression tree is as part of a BlockExpression
            // other expression types cannot contain a void-returning expression
            case not null when node.Type == typeof(void):
                foreach (var x in ExpressionStack)
                {
                    // BlockExpressions may have a value even though one of the statements is a void-returning expression
                    if (x is BlockExpression && node != x) { break; }

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
                    if (Evaluable.ContainsKey(x)) { break; }

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

        ExpressionStack.Pop();

        return ret;
    }

    private (bool Evaluated, FormattableString Value) GetValue(Expression node)
    {
        if (!Evaluable.TryGetValue(node, out bool canEvaluate))
        {
            Visit(node);
            Evaluable.TryGetValue(node, out canEvaluate);
        }

        if (!canEvaluate)
        {
            return (false, $"");
        }

        var lambdaExpression = Expression.Lambda(node);
        return (canEvaluate, $"{lambdaExpression.Compile().DynamicInvoke()}");
    }
}
