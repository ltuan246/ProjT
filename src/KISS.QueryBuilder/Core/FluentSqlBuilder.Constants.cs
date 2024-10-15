namespace KISS.QueryBuilder.Core;

/// <summary>
///     Declares const for the <see cref="FluentSqlBuilder{TEntity}" /> type.
/// </summary>
/// <typeparam name="TEntity">The type of results to return.</typeparam>
public sealed partial class FluentSqlBuilder<TEntity>
{
    /// <summary>
    ///     Used to address <typeparamref name="TEntity"/>.
    /// </summary>
    private const string DefaultEntityAliasTemplate = "Extend";

    /// <summary>
    ///     Represents a list of names that identify the columns of the final result table.
    /// </summary>
    private const string TemporaryColumnsTemplate = "{columns}";

    /// <summary>
    ///     Comma-separated string.
    /// </summary>
    private const char Comma = ',';

    /// <summary>
    ///     Used to begin parenthetical text.
    /// </summary>
    private const char OpenParenthesis = '(';

    /// <summary>
    ///     Used to end parenthetical text.
    /// </summary>
    private const char CloseParenthesis = ')';
}
