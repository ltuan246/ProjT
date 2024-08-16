namespace KISS.FluentQueryBuilder.Core;

public interface IQueryExpressionVisitor
{
    void Visit(Expression expression);
    void Visit(BinaryExpression expression);
    void Visit(MemberExpression expression);
    void Visit(ConstantExpression expression);
}
