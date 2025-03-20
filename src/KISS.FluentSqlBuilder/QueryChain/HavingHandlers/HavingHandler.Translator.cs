namespace KISS.FluentSqlBuilder.QueryChain.HavingHandlers;

/// <summary>
///     A handler for processing HAVING clauses in a query chain.
///     This class provides the translation logic for converting aggregate conditions
///     into SQL-compatible form.
/// </summary>
public sealed partial record HavingHandler
{
    /// <summary>
    ///     Translates a member expression into SQL for HAVING conditions.
    ///     Handles property and field access within aggregate conditions.
    /// </summary>
    /// <param name="memberExpression">The member expression to translate.</param>
    protected override void Translate(MemberExpression memberExpression)
    {
        if (memberExpression is { Expression: ParameterExpression parameterExpression })
        {
            Append($"{Composite.GetAliasMapping(parameterExpression.Type)}_{memberExpression.Member.Name}");
        }
    }

    /// <summary>
    ///     Translates a constant expression into SQL for HAVING conditions.
    ///     Converts constant values into their string representation for SQL.
    /// </summary>
    /// <param name="constantExpression">The constant expression to translate.</param>
    protected override void Translate(ConstantExpression constantExpression)
        => AppendFormat($"{constantExpression.Value}");

    /// <summary>
    ///     Translates a binary expression into SQL for HAVING conditions.
    ///     Handles mathematical and comparison operations within aggregate conditions.
    /// </summary>
    /// <param name="binaryExpression">The binary expression to translate.</param>
    protected override void Translate(BinaryExpression binaryExpression)
    {
        Translate(binaryExpression.Left);
        Append(BinaryOperandMap[binaryExpression.NodeType]);
        Translate(binaryExpression.Right);
    }

    /// <summary>
    ///     Translates a unary expression into SQL for HAVING conditions.
    ///     Handles operations like negation and type conversion.
    /// </summary>
    /// <param name="unaryExpression">The unary expression to translate.</param>
    protected override void Translate(UnaryExpression unaryExpression)
    {
        if (unaryExpression is { Operand: { } expression })
        {
            Translate(expression);
        }
    }

    /// <summary>
    ///     Translates a lambda expression into SQL for HAVING conditions.
    ///     Processes the body of lambda expressions used in aggregate conditions.
    /// </summary>
    /// <param name="lambdaExpression">The lambda expression to translate.</param>
    protected override void Translate(LambdaExpression lambdaExpression)
    {
        if (lambdaExpression is { Body: { } expression })
        {
            Translate(expression);
        }
    }

    /// <summary>
    ///     Translates a method call expression into SQL for HAVING conditions.
    ///     Handles SQL function calls and custom aggregate methods.
    /// </summary>
    /// <param name="methodCallExpression">The method call expression to translate.</param>
    protected override void Translate(MethodCallExpression methodCallExpression)
    {
        if (methodCallExpression is { Arguments: [{ } expression] })
        {
            Append($"{methodCallExpression.Method.Name}(");
            Translate(expression);
            Append(")");
        }
    }
}
