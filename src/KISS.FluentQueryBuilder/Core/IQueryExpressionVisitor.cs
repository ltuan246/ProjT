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
    /// <returns>The expression being visited, or an expression which should replace it in the tree.</returns>
    Expression Visit(Expression expression);

    /// <summary>
    ///     Visits the children of the BinaryExpression.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    /// <returns>The expression being visited, or an expression which should replace it in the tree.</returns>
    Expression Visit(BinaryExpression expression);

    /// <summary>
    ///     Visits the children of the MemberExpression.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    /// <returns>The expression being visited, or an expression which should replace it in the tree.</returns>
    Expression Visit(MemberExpression expression);

    /// <summary>
    ///     Visits the children of the ConstantExpression.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    /// <returns>The expression being visited, or an expression which should replace it in the tree.</returns>
    Expression Visit(ConstantExpression expression);

    /// <summary>
    ///     Visits the children of the NewExpression.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    /// <returns>The expression being visited, or an expression which should replace it in the tree.</returns>
    Expression Visit(NewExpression expression);

    /// <summary>
    ///     Visits the children of the MethodCallExpression.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    /// <returns>The expression being visited, or an expression which should replace it in the tree.</returns>
    Expression Visit(MethodCallExpression expression);

    /// <summary>
    ///     Visits the children of the ParameterExpression.
    /// </summary>
    /// <param name="expression">The nodes to visit.</param>
    /// <returns>The expression being visited, or an expression which should replace it in the tree.</returns>
    Expression Visit(ParameterExpression expression);
}
