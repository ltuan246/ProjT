namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder
{
    private void AppendFormattable(FormattableString formattable)
        => _ = formattable.ArgumentCount switch
        {
            0 => StateBuilder.Append(formattable.Format),
            _ => StateBuilder.AppendFormat(Formatter, formattable.Format, formattable.GetArguments())
        };

    // public void AppendFormatted<T>(T value)
    //     => Builder.Append(SqlFormatter.Format(value));

    // public void AppendLiteral(string value)
    //     => Builder.Append(value);

    // public void EndClauseAction()
    //     => CloseOpenParentheses();

    // public bool IsClauseActionEnabled(ClauseAction clauseAction)
    //     => CanAppendClause(clauseAction);

    // public void StartClauseAction(ClauseAction clauseAction)
    //     => AppendClause(clauseAction);
}