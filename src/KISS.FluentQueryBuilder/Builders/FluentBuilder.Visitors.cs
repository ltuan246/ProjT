namespace KISS.FluentQueryBuilder.Builders;

/// <summary>
///     Implements <see cref="IQueryExpressionVisitor" /> interfaces for the <see cref="FluentBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
public sealed partial record FluentBuilder<TEntity> : IQueryExpressionVisitor
{
    /// <inheritdoc />
    public Expression Visit(Expression expression)
        => expression switch
        {
            BinaryExpression binaryExpression => Visit(binaryExpression),
            MemberExpression memberExpression => Visit(memberExpression),
            ConstantExpression constantExpression => Visit(constantExpression),
            NewExpression newExpression => Visit(newExpression),
            MethodCallExpression methodCallExpression => Visit(methodCallExpression),
            ParameterExpression parameterExpression => Visit(parameterExpression),
            _ => expression
        };

    /// <inheritdoc />
    public Expression Visit(BinaryExpression binaryExpression)
    {
        var left = Visit(binaryExpression.Left);
        var right = Visit(binaryExpression.Right);

        return binaryExpression.Update(left, binaryExpression.Conversion, right);
    }

    /// <inheritdoc />
    public Expression Visit(MemberExpression memberExpression)
    {
        if (memberExpression.Expression is not null)
        {
            var expression = Visit(memberExpression.Expression);
            return memberExpression.Update(expression);
        }

        return memberExpression;
    }

    /// <inheritdoc />
    public Expression Visit(ConstantExpression constantExpression)
        => constantExpression;

    /// <inheritdoc />
    public Expression Visit(NewExpression newExpression)
    {
        if (newExpression.Arguments.Count == 0)
        {
            return newExpression;
        }

        var arguments = new Expression[newExpression.Arguments.Count];
        for (var i = 0; i < arguments.Length; i++)
        {
            arguments[i] = Visit(newExpression.Arguments[i]);
        }

        return newExpression.Update(arguments);
    }

    /// <inheritdoc />
    public Expression Visit(MethodCallExpression methodCallExpression)
        => methodCallExpression;

    /// <inheritdoc />
    public Expression Visit(ParameterExpression parameterExpression)
        => parameterExpression;
}
