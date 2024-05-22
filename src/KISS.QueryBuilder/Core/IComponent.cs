namespace KISS.QueryBuilder.Core;

public interface IComponent
{
    void Accept(IVisitor visitor);
}