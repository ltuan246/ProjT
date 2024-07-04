namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder
{
    private StringBuilder Builder { get; } = new();

    private List<ClauseAction> ClauseActions { get; } = [];

    private SqlFormatter SqlFormatter { get; } = new();

    private bool HasOpenParentheses { get; set; } = false;

    private void Join(string separator, IEnumerable<IComponent> expressions)
    {
        using IEnumerator<IComponent> enumerator = expressions.GetEnumerator();
        if (enumerator.MoveNext())
        {
            enumerator.Current.Accept(this);
            while (enumerator.MoveNext())
            {
                Builder.Append(separator);
                enumerator.Current.Accept(this);
            }
        }
    }

    private bool CanAppendClause(ClauseAction clauseAction)
    {
        return !ClauseActions.Exists(c => c is ClauseAction.Delete or ClauseAction.Update) ||
            clauseAction is not ClauseAction.GroupBy and not ClauseAction.Having and not ClauseAction.OrderBy
            and not ClauseAction.FetchNext and not ClauseAction.Limit and not ClauseAction.Offset
            and not ClauseAction.Rows and not ClauseAction.Only;
    }

    private void CloseOpenParentheses()
    {
        if (!HasOpenParentheses)
        {
            return;
        }

        Builder.Append(ClauseConstants.CloseParentheses);
        HasOpenParentheses = false;
    }

    private void AppendWhere(bool isFilter = false)
    {
        if (ClauseActions.Contains(ClauseAction.Where))
        {
            Builder
                // .Append(Constants.Space)
                .Append(ClauseConstants.Where.AndSeparator);
        }
        else
        {
            ClauseActions.Add(ClauseAction.Where);
            Builder
                .AppendLine()
                .Append(ClauseConstants.Where.Clause);
        }

        // Builder.Append(Constants.Space);

        if (!isFilter)
        {
            return;
        }

        Builder.Append(ClauseConstants.OpenParentheses);
        HasOpenParentheses = true;
    }
}