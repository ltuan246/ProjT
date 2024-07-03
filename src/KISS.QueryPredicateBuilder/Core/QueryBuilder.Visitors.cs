namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder : IVisitor
{
    public void Visit(ConcreteComponentA element)
    {
        Builder.Append("ConcreteComponentA");
    }

    public void Visit(ConcreteComponentB element)
    {
        Builder.Append("ConcreteComponentB");
    }

    public void Visit(OperatorFilterDefinition element)
    {
        Builder.Append("OperatorFilterDefinition");
    }
}