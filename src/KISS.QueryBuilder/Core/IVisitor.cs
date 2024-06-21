namespace KISS.QueryBuilder.Core;

public interface IVisitor
{
    void Visit(IQuerying concreteQuerying);

    void Visit(IQueryBuilders queryBuilders);

    void Visit(IFilterDefinition filterDefinition);

    void Visit(ISingleItemAsArrayOperatorFilterDefinition operatorFilterDefinition);

    void Visit(IMultipleFiltersDefinition multipleFiltersDefinition);

    void Visit(ISortDefinition filterDefinition);

    void Visit(IMultipleSortsDefinition sorts);
}