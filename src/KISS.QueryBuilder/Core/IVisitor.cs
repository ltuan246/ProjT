namespace KISS.QueryBuilder.Core;

public interface IVisitor
{
    void Visit(IComponent concreteComponent);
}