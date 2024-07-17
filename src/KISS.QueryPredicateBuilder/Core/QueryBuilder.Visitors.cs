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
        PushState(ClauseAction.Select);
        AppendFormattable(element.Formattable);
        PopState();
    }

    public void Visit(OffsetDefinition element)
    {
        PushState(ClauseAction.Offset);
        AppendFormattable(element.Formattable);
        PopState();
    }

    public void Visit(FetchDefinition element)
    {
        PushState(ClauseAction.FetchNext);
        AppendFormattable(element.Formattable);
        PopState();
    }

    public void Visit(OrderByDefinition element)
    {
        PushState(ClauseAction.OrderBy);
        AppendFormattable(element.Formattable);
        PopState();
    }
}