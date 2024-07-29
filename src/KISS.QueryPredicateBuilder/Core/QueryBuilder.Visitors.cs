namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder : IVisitor
{
    public void Visit([NotNull] OperatorFilterDefinition element)
    {
        PushState(ClauseAction.Where);
        AppendFormatString(element.ComparisonOperator);
        PopState();
    }

    public void Visit([NotNull] SingleItemAsArrayOperatorFilterDefinition element)
    {
        PushState(ClauseAction.Where);
        AppendFormatString(element.ContainsClause);
        PopState();
    }

    public void Visit([NotNull] RangeFilterDefinition element)
    {
        PushState(ClauseAction.Where);
        AppendFormatString(element.BetweenOperator);
        PopState();
    }

    public void Visit([NotNull] CombinedFilterDefinition element)
    {
        PushState(element.Clause, element.Operators.Count);
        Join(element.Separator, element.Operators);
        PopState();
    }

    public void Visit([NotNull] ProjectionDefinition element)
    {
        PushState(ClauseAction.Select);
        AppendFormatString(element.ProjectionClause);
        PopState();
    }

    public void Visit([NotNull] OffsetDefinition element)
    {
        PushState(ClauseAction.Offset);
        AppendFormatString(element.OffsetClause);
        PopState();
    }

    public void Visit([NotNull] FetchDefinition element)
    {
        PushState(ClauseAction.FetchNext);
        AppendFormatString(element.FetchClause);
        PopState();
    }

    public void Visit([NotNull] OrderByDefinition element)
    {
        PushState(ClauseAction.OrderBy);
        AppendFormatString(element.OrderByClause);
        PopState();
    }
}