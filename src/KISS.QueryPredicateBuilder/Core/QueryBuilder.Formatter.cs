namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder
{
    public void AppendFormatted<T>(T value)
        => Builder.Append(SqlFormatter.Format(value));

    public void AppendLiteral(string value)
        => Builder.Append(value);

    public void EndClauseAction()
        => CloseOpenParentheses();

    public bool IsClauseActionEnabled(ClauseAction clauseAction)
        => CanAppendClause(clauseAction);

    // public void StartClauseAction(ClauseAction clauseAction)
    //     => AppendClause(clauseAction);
}