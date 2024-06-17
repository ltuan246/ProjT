namespace KISS.QueryBuilder.Core;

public interface IVisitor
{
    void Visit(IQuerying concreteQuerying);

    void Visit(IFilterDefinition filterDefinition);

    void Visit(ISingleItemAsArrayOperatorFilterDefinition operatorFilterDefinition);

    void Visit(IGroupingFilterDefinition groupingFilterDefinition);
}