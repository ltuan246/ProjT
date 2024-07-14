namespace KISS.QueryPredicateBuilder.Core;

public interface IVisitor
{
    void Visit(OperatorFilterDefinition element);

    void Visit(SingleItemAsArrayOperatorFilterDefinition element);

    void Visit(RangeFilterDefinition element);

    void Visit(CombinedFilterDefinition element);

    void Visit(ProjectionDefinition element);
}