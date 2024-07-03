namespace KISS.QueryPredicateBuilder.Core;

public interface IVisitor
{
    void Visit(ConcreteComponentA element);

    void Visit(ConcreteComponentB element);

    void Visit(OperatorFilterDefinition element);
}