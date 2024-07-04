namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder : IVisitor
{
    public void Visit(OperatorFilterDefinition element)
    {
        Builder.Append("OperatorFilterDefinition");
    }

    public void Visit(SingleItemAsArrayOperatorFilterDefinition element)
    {
        Builder.Append("OperatorFilterDefinition");
    }

    public void Visit(RangeFilterDefinition element)
    {
        Builder.Append("OperatorFilterDefinition");
    }

    public void Visit(CombinedFilterDefinition element)
    {
        Join(element.Separator, element.Components);
    }
}