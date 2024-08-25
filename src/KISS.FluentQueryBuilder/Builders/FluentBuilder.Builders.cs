namespace KISS.FluentQueryBuilder.Builders;

public sealed partial record FluentBuilder<TEntity>
{
    /// <summary>
    ///     The Action clause defines the SQL statements that are executed when the trigger is activated.
    /// </summary>
    private enum ClauseAction
    {
        // None,
        // Delete,
        // FetchNext,
        // GroupBy,
        // Having,
        // InnerJoin,
        // Insert,
        // InsertColumn,
        // InsertValue,
        // Limit,
        // LeftJoin,
        // Offset,
        // Only,
        // OrderBy,
        // RightJoin,
        // Rows,
        Select,
        SelectDistinct,

        // SelectFrom,
        // Update,
        // UpdateSet,
        Where
    }

    private StringBuilder StringBuilder { get; } = new();

    private SqlFormatter SqlFormatter { get; } = new();

    private Stack<BuildState> State { get; } = new();

    private Dictionary<ClauseAction, Expression> EntryClause { get; } = new();

    /// <summary>
    ///     The SQL to execute for the query.
    /// </summary>
    private string Sql
        => StringBuilder.ToString();

    /// <summary>
    ///     The parameters to pass, if any.
    /// </summary>
    private object Parameters
        => SqlFormatter.Parameters;

    private void PushState(Expression expression)
    {
        BuildState newState = new() { Expression = expression };
        State.Push(newState);
    }

    private void PopState() => _ = State.Pop();

    private void OpenParentheses()
    {
        const char openParentheses = '(';
        StringBuilder.Append(openParentheses);
    }

    private void CloseParentheses()
    {
        const char closeParentheses = ')';
        StringBuilder.Append(closeParentheses);
    }

    private void SetEntryClause(ClauseAction clauseAction, Expression expression) =>
        EntryClause.Add(clauseAction, expression);

    private sealed class BuildState
    {
        public required Expression Expression { get; init; }
    }
}
