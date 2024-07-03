namespace KISS.QueryPredicateBuilder.Core;

public interface IComponent
{
    void Accept(IVisitor visitor);
}
