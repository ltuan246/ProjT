namespace KISS.FluentQueryBuilder.Core;

/// <summary>
///     An interface that visits a SQL expression tree.
/// </summary>
public interface IQueryExpressionVisitor
{
    /// <summary>
    ///     Dispatches the expression to one of the more specialized visit methods in this class.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    void Visit(Expression expression);

    /// <summary>
    ///     Visits the children of the BinaryExpression.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    void Visit(BinaryExpression expression);

    /// <summary>
    ///     Visits the children of the MemberExpression.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    void Visit(MemberExpression expression);

    /// <summary>
    ///     Visits the children of the ConstantExpression.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    void Visit(ConstantExpression expression);

    /// <summary>
    ///     Visits the children of the NewExpression.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    void Visit(NewExpression expression);

    /// <summary>
    ///     Visits the children of the MethodCallExpression.
    /// </summary>
    /// <param name="methodCallExpression">The nodes to visit.</param>
    void Visit(MethodCallExpression methodCallExpression);
}
