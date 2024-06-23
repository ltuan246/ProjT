namespace KISS.QueryBuilder.Core;

public interface IVisitor
{
    void Visit(IQuerying concreteQuerying);

    void Visit(IBuilder builder);

    void Visit(IOperatorFilterDefinition operatorFilterDefinition);

    void Visit(ISingleItemAsArrayOperatorFilterDefinition operatorFilterDefinition);

    void Visit(IRangeFilterDefinition rangeFilterDefinition);

    void Visit(IMultipleFiltersDefinition multipleFiltersDefinition);

    void Visit(ISortDefinition filterDefinition);

    void Visit(IMultipleSortsDefinition sorts);

    void Visit(ISingleFieldProjectionDefinition singleFieldProjection);

    void Visit(ISliceProjectionDefinition singleFieldProjection);

    void Visit(ICombinedProjectionDefinition singleFieldProjection);
}