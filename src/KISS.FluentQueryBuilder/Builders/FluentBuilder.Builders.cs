namespace KISS.FluentQueryBuilder.Builders;

/// <summary>
///     Declares specifies methods for creating the different parts of the <see cref="FluentBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of the record.</typeparam>
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

    private Dictionary<ClauseAction, Expression> EntryClause { get; } = [];

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
        => State.Push(new() { Expression = expression });

    private void PopState()
        => State.Pop();

    private void OpenParentheses()
        => StringBuilder.Append(BuilderConstants.OpenParentheses);

    private void CloseParentheses()
        => StringBuilder.Append(BuilderConstants.CloseParentheses);

    private void SetEntryClause(ClauseAction clauseAction, Expression expression)
        => EntryClause.Add(clauseAction, expression);

    private void AddCommaSeparated()
        => StringBuilder.Append(BuilderConstants.Comma);

    private sealed class BuildState
    {
        public required Expression Expression { get; init; }
    }
}
