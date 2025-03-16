namespace KISS.FluentSqlBuilder.QueryChain.HavingHandlers;

/// <summary>
///     A handler for processing <c>HAVING</c> in a query chain.
/// </summary>
public sealed partial record HavingHandler
{
    private Dictionary<ExpressionType, string> BinaryOperandMap { get; } = new()
    {
        { ExpressionType.Assign, " = " },
        { ExpressionType.Equal, " = " },
        { ExpressionType.NotEqual, " != " },
        { ExpressionType.GreaterThan, " > " },
        { ExpressionType.GreaterThanOrEqual, " >= " },
        { ExpressionType.LessThan, " < " },
        { ExpressionType.LessThanOrEqual, " <= " },
        { ExpressionType.OrElse, " OR " },
        { ExpressionType.AndAlso, " AND " },
        { ExpressionType.Coalesce, " ?? " },
        { ExpressionType.Add, " + " },
        { ExpressionType.Subtract, " - " },
        { ExpressionType.Multiply, " * " },
        { ExpressionType.Divide, " / " },
        { ExpressionType.Modulo, " % " },
        { ExpressionType.And, " & " },
        { ExpressionType.Or, " | " },
        { ExpressionType.ExclusiveOr, " ^ " },
        { ExpressionType.LeftShift, " << " },
        { ExpressionType.RightShift, " >> " }
    };

    /// <inheritdoc />
    protected override void Translate(MemberExpression memberExpression)
    {
        if (memberExpression is { Expression: ParameterExpression parameterExpression })
        {
            Append($"{Composite.GetAliasMapping(parameterExpression.Type)}_{memberExpression.Member.Name}");
        }
    }

    /// <inheritdoc />
    protected override void Translate(ConstantExpression constantExpression)
        => AppendFormat($"{constantExpression.Value}");

    /// <inheritdoc />
    protected override void Translate(BinaryExpression binaryExpression)
    {
        Translate(binaryExpression.Left);
        Append(BinaryOperandMap[binaryExpression.NodeType]);
        Translate(binaryExpression.Right);
    }

    /// <inheritdoc />
    protected override void Translate(UnaryExpression unaryExpression)
    {
        if (unaryExpression is { Operand: Expression expression })
        {
            Translate(expression);
        }
    }

    /// <inheritdoc />
    protected override void Translate(LambdaExpression lambdaExpression)
    {
        if (lambdaExpression is { Body: Expression expression })
        {
            Translate(expression);
        }
    }

    /// <inheritdoc />
    protected override void Translate(MethodCallExpression methodCallExpression)
    {
        if (methodCallExpression is { Arguments: [Expression expression] })
        {
            Append($"{methodCallExpression.Method.Name}(");
            Translate(expression);
            Append(")");
        }
    }
}