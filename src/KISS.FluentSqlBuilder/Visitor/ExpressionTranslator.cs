namespace KISS.FluentSqlBuilder.Visitor;

/// <summary>
///     A custom expression visitor that translates LINQ expressions into SQL-compatible syntax.
///     This class provides the foundation for converting C# expressions into SQL statements
///     by implementing a visitor pattern for different types of expressions.
/// </summary>
public abstract record ExpressionTranslator
{
    /// <summary>
    ///     Maps C# expression types to their corresponding SQL operators and symbols.
    ///     This dictionary provides the translation rules for converting C# expressions
    ///     into SQL-compatible syntax.
    /// </summary>
    protected Dictionary<ExpressionType, string> BinaryOperandMap { get; } = new()
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

    /// <summary>
    ///     Dispatches the expression to the appropriate specialized visit method based on its type.
    ///     This method serves as the entry point for expression translation.
    /// </summary>
    /// <param name="expression">The expression to translate.</param>
    protected void Translate(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression binaryExpression:
                Translate(binaryExpression);
                break;

            case UnaryExpression unaryExpression:
                Translate(unaryExpression);
                break;

            case MemberExpression memberExpression:
                Translate(memberExpression);
                break;

            case ConstantExpression constantExpression:
                Translate(constantExpression);
                break;

            case NewExpression newExpression:
                Translate(newExpression);
                break;

            case MemberInitExpression memberInitExpression:
                Translate(memberInitExpression);
                break;

            case MethodCallExpression methodCallExpression:
                Translate(methodCallExpression);
                break;

            case LambdaExpression lambdaExpression:
                Translate(lambdaExpression);
                break;
        }
    }

    /// <summary>
    ///     Translates a binary expression (e.g., x => x.Age > 18) into SQL syntax.
    /// </summary>
    /// <param name="binaryExpression">The binary expression to translate.</param>
    protected virtual void Translate(BinaryExpression binaryExpression) { }

    /// <summary>
    ///     Translates a unary expression (e.g., x => !x.IsActive) into SQL syntax.
    /// </summary>
    /// <param name="unaryExpression">The unary expression to translate.</param>
    protected virtual void Translate(UnaryExpression unaryExpression) { }

    /// <summary>
    ///     Translates a member expression (e.g., x => x.Name) into SQL syntax.
    /// </summary>
    /// <param name="memberExpression">The member expression to translate.</param>
    protected virtual void Translate(MemberExpression memberExpression) { }

    /// <summary>
    ///     Translates a constant expression (e.g., 42, "Hello") into SQL syntax.
    /// </summary>
    /// <param name="constantExpression">The constant expression to translate.</param>
    protected virtual void Translate(ConstantExpression constantExpression) { }

    /// <summary>
    ///     Translates a new expression (e.g., new { x.Name, x.Age }) into SQL syntax.
    /// </summary>
    /// <param name="newExpression">The new expression to translate.</param>
    protected virtual void Translate(NewExpression newExpression) { }

    /// <summary>
    ///     Translates a member initialization expression into SQL syntax.
    /// </summary>
    /// <param name="memberInitExpression">The member initialization expression to translate.</param>
    protected virtual void Translate(MemberInitExpression memberInitExpression) { }

    /// <summary>
    ///     Translates a method call expression (e.g., x => x.ToString()) into SQL syntax.
    /// </summary>
    /// <param name="methodCallExpression">The method call expression to translate.</param>
    protected virtual void Translate(MethodCallExpression methodCallExpression) { }

    /// <summary>
    ///     Translates a lambda expression into SQL syntax.
    /// </summary>
    /// <param name="lambdaExpression">The lambda expression to translate.</param>
    protected virtual void Translate(LambdaExpression lambdaExpression) { }
}
