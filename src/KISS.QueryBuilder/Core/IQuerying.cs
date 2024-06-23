namespace KISS.QueryBuilder.Core;

public interface IQuerying
{
    void Accept(IVisitor visitor);
}