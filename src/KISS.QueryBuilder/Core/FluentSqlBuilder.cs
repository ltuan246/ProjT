namespace KISS.QueryBuilder.Core;

/// <summary>
///     A class that defines the fluent SQL builder type. The core <see cref="FluentSqlBuilder{TEntity}" /> partial class.
/// </summary>
/// <typeparam name="TEntity">The type of results to return.</typeparam>
public sealed partial class FluentSqlBuilder<TEntity>
{
    private StringBuilder SqlBuilder { get; } = new();

    private SqlFormatter SqlFormatter { get; } = new();

    /// <summary>
    ///     Use checks to know when to use Close Parenthesis.
    /// </summary>
    private bool HasOpenParentheses { get; set; }

    private void OpenParentheses()
    {
        HasOpenParentheses = true;
        SqlBuilder.Append(BuilderConstants.OpenParentheses);
    }

    private void CloseParentheses()
    {
        if (!HasOpenParentheses)
        {
            return;
        }

        HasOpenParentheses = false;
        SqlBuilder.Append(BuilderConstants.CloseParentheses);
    }

    private void AddCommaSeparated()
        => SqlBuilder.Append(BuilderConstants.Comma);
}
