namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder : IVisitor
{
    public void Visit(OperatorFilterDefinition element)
    {
        PushState(ClauseAction.Where);
        AppendFormattable(element.Formattable);
        PopState();
    }

    public void Visit(SingleItemAsArrayOperatorFilterDefinition element)
    {
        PushState(ClauseAction.Where);
        AppendFormattable(element.Formattable);
        PopState();
    }

    public void Visit(RangeFilterDefinition element)
    {
        PushState(ClauseAction.Where);
        AppendFormattable(element.Formattable);
        PopState();
    }

    public void Visit(CombinedFilterDefinition element)
    {
        PushState(element.Clause, element.Components.Length);
        Join(element.Separator, element.Components);
        PopState();
    }

    public void Visit(ProjectionDefinition element)
    {
    }
}