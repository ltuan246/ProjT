namespace KISS.FluentSqlBuilder.Visitor;

/// <summary>
///     A custom expression visitor that translates LINQ expressions into SQL-compatible syntax.
/// </summary>
public abstract record ExpressionTranslator
{
    /// <summary>
    ///     Dispatches the expression to one of the more specialized visit methods in this class.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    public void Translate(Expression expression)
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
        }
    }

    /// <summary>
    ///     Visits the children of the BinaryExpression.
    /// </summary>
    /// <param name="binaryExpression">The nodes to visit.</param>
    protected virtual void Translate(BinaryExpression binaryExpression) { }

    /// <summary>
    ///     Visits the children of the UnaryExpression.
    /// </summary>
    /// <param name="unaryExpression">The nodes to visit.</param>
    protected virtual void Translate(UnaryExpression unaryExpression) { }

    /// <summary>
    ///     Visits the children of the MemberExpression.
    /// </summary>
    /// <param name="memberExpression">The nodes to visit.</param>
    protected virtual void Translate(MemberExpression memberExpression) { }

    /// <summary>
    ///     Visits the children of the ConstantExpression.
    /// </summary>
    /// <param name="constantExpression">The nodes to visit.</param>
    protected virtual void Translate(ConstantExpression constantExpression) { }

    /// <summary>
    ///     Visits the children of the NewExpression.
    /// </summary>
    /// <param name="newExpression">The nodes to visit.</param>
    protected virtual void Translate(NewExpression newExpression) { }

    /// <summary>
    ///     Visits the children of the MemberInitExpression.
    /// </summary>
    /// <param name="memberInitExpression">The nodes to visit.</param>
    protected virtual void Translate(MemberInitExpression memberInitExpression) { }

    /// <summary>
    ///     Visits the children of the MethodCallExpression.
    /// </summary>
    /// <param name="methodCallExpression">The nodes to visit.</param>
    protected virtual void Translate(MethodCallExpression methodCallExpression) { }
}
