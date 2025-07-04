namespace KISS.FluentSqlBuilder.Visitor;

/// <summary>
///     An abstract record that serves as a base for visiting and processing expression trees
///     using the visitor pattern. It dispatches expressions to specific visit methods based
///     on their type, allowing derived classes to implement custom processing logic for
///     different expression types.
/// </summary>
public abstract record SimpleExpressionVisitor
{
    /// <summary>
    ///     Dispatches the expression to the appropriate specialized visit method based on its type.
    ///     This method serves as the entry point for expression processing and enables
    ///     extensibility for custom expression handling in derived classes.
    /// </summary>
    /// <param name="expression">The expression to process, or null if no expression is provided.</param>
    protected virtual void Visit(Expression? expression)
    {
        switch (expression)
        {
            case BinaryExpression binaryExpression:
                Visit(binaryExpression);
                break;
            case UnaryExpression unaryExpression:
                Visit(unaryExpression);
                break;
            case MemberExpression memberExpression:
                Visit(memberExpression);
                break;
            case ConstantExpression constantExpression:
                Visit(constantExpression);
                break;
            case NewExpression newExpression:
                Visit(newExpression);
                break;
            case MemberInitExpression memberInitExpression:
                Visit(memberInitExpression);
                break;
            case MethodCallExpression methodCallExpression:
                Visit(methodCallExpression);
                break;
            case LambdaExpression lambdaExpression:
                Visit(lambdaExpression);
                break;
        }
    }

    /// <summary>
    ///     Processes a binary expression (e.g., x => x.Age > 18). Derived classes can override
    ///     this method to implement custom logic for binary expressions such as comparisons,
    ///     logical operations, and arithmetic operations.
    /// </summary>
    /// <param name="binaryExpression">The binary expression to process.</param>
    protected virtual void Visit(BinaryExpression binaryExpression) { }

    /// <summary>
    ///     Processes a unary expression (e.g., x => !x.IsActive). Derived classes can override
    ///     this method to implement custom logic for unary expressions such as negation,
    ///     type conversion, or other single-operand operations.
    /// </summary>
    /// <param name="unaryExpression">The unary expression to process.</param>
    protected virtual void Visit(UnaryExpression unaryExpression) { }

    /// <summary>
    ///     Processes a member expression (e.g., x => x.Name). Derived classes can override
    ///     this method to implement custom logic for member expressions, including property
    ///     and field access.
    /// </summary>
    /// <param name="memberExpression">The member expression to process.</param>
    protected virtual void Visit(MemberExpression memberExpression) { }

    /// <summary>
    ///     Processes a constant expression (e.g., 42, "Hello"). Derived classes can override
    ///     this method to implement custom logic for constant expressions, such as embedding
    ///     literal values in SQL or other output.
    /// </summary>
    /// <param name="constantExpression">The constant expression to process.</param>
    protected virtual void Visit(ConstantExpression constantExpression) { }

    /// <summary>
    ///     Processes a new expression (e.g., new { x.Name, x.Age }). Derived classes can override
    ///     this method to implement custom logic for new expressions, such as anonymous type
    ///     construction or object instantiation.
    /// </summary>
    /// <param name="newExpression">The new expression to process.</param>
    protected virtual void Visit(NewExpression newExpression) { }

    /// <summary>
    ///     Processes a member initialization expression (e.g., new X { Prop = value }). Derived classes can override
    ///     this method to implement custom logic for member initialization expressions, supporting
    ///     object initializers and complex construction scenarios.
    /// </summary>
    /// <param name="memberInitExpression">The member initialization expression to process.</param>
    protected virtual void Visit(MemberInitExpression memberInitExpression) { }

    /// <summary>
    ///     Processes a method call expression (e.g., x => x.ToString()). Derived classes can override
    ///     this method to implement custom logic for method call expressions, enabling translation
    ///     of method invocations to SQL or other representations.
    /// </summary>
    /// <param name="methodCallExpression">The method call expression to process.</param>
    protected virtual void Visit(MethodCallExpression methodCallExpression) { }

    /// <summary>
    ///     Processes a lambda expression. Derived classes can override
    ///     this method to implement custom logic for lambda expressions, such as extracting
    ///     parameters or translating the lambda body.
    /// </summary>
    /// <param name="lambdaExpression">The lambda expression to process.</param>
    protected virtual void Visit(LambdaExpression lambdaExpression) { }
}
