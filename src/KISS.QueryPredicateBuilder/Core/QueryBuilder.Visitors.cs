namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder : IVisitor
{
    public void Visit(OperatorFilterDefinition element)
    {
        PushState(ClauseAction.Where);
        AppendFormatString(element.ComparisonOperator);
        PopState();
    }

    public void Visit(SingleItemAsArrayOperatorFilterDefinition element)
    {
        PushState(ClauseAction.Where);
        AppendFormatString(element.ContainsClause);
        PopState();
    }

    public void Visit(RangeFilterDefinition element)
    {
        PushState(ClauseAction.Where);
        AppendFormatString(element.BetweenOperator);
        PopState();
    }

    public void Visit(CombinedFilterDefinition element)
    {
        PushState(element.Clause, element.Operators.Length);
        Join(element.Separator, element.Operators);
        PopState();
    }

    public void Visit(ProjectionDefinition element)
    {
        PushState(ClauseAction.Select);
        AppendFormatString(element.Formattable);
        PopState();
    }

    public void Visit(OffsetDefinition element)
    {
        PushState(ClauseAction.Offset);
        AppendFormatString(element.OffsetClause);
        PopState();
    }

    public void Visit(FetchDefinition element)
    {
        PushState(ClauseAction.FetchNext);
        AppendFormatString(element.FetchClause);
        PopState();
    }

    public void Visit(OrderByDefinition element)
    {
        PushState(ClauseAction.OrderBy);
        AppendFormatString(element.OrderByClause);
        PopState();
    }
}