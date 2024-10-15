namespace KISS.QueryPredicateBuilder.Core;

/// <summary>
///     The Visitor implements several versions of the same behaviors, tailored for different concrete element classes.
/// </summary>
public sealed partial class QueryBuilder : IVisitor
{
    /// <inheritdoc />
    public void Visit([NotNull] OperatorFilterDefinition element)
    {
        PushState(ClauseAction.Where);
        AppendFormatString(element.ComparisonOperator);
        PopState();
    }

    /// <inheritdoc />
    public void Visit([NotNull] SingleItemAsArrayOperatorFilterDefinition element)
    {
        PushState(ClauseAction.Where);
        AppendFormatString(element.ContainsClause);
        PopState();
    }

    /// <inheritdoc />
    public void Visit([NotNull] RangeFilterDefinition element)
    {
        PushState(ClauseAction.Where);
        AppendFormatString(element.BetweenOperator);
        PopState();
    }

    /// <inheritdoc />
    public void Visit([NotNull] CombinedFilterDefinition element)
    {
        PushState(element.Clause, element.Operators.Count);
        Join(element.Separator, element.Operators);
        PopState();
    }

    /// <inheritdoc />
    public void Visit([NotNull] ProjectionDefinition element)
    {
        PushState(ClauseAction.Select);
        AppendFormatString(element.ProjectionClause);
        PopState();
    }

    /// <inheritdoc />
    public void Visit([NotNull] OffsetDefinition element)
    {
        PushState(ClauseAction.Offset);
        AppendFormatString(element.OffsetClause);
        PopState();
    }

    /// <inheritdoc />
    public void Visit([NotNull] FetchDefinition element)
    {
        PushState(ClauseAction.FetchNext);
        AppendFormatString(element.FetchClause);
        PopState();
    }

    /// <inheritdoc />
    public void Visit([NotNull] OrderByDefinition element)
    {
        PushState(ClauseAction.OrderBy);
        AppendFormatString(element.OrderByClause);
        PopState();
    }
}
