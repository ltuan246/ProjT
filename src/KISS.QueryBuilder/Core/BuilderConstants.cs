namespace KISS.QueryBuilder.Core;

/// <summary>
///     Declares common constants for the <see cref="FluentSqlBuilder{TEntity}" /> type.
/// </summary>
internal static class BuilderConstants
{
    /// <summary>
    ///     Comma-separated string.
    /// </summary>
    internal const char Comma = ',';

    /// <summary>
    ///     And-separated string.
    /// </summary>
    internal const string AndSeparator = "AND";

    /// <summary>
    ///     Used to begin parenthetical text.
    /// </summary>
    internal const char OpenParentheses = '(';

    /// <summary>
    ///     Used to end parenthetical text.
    /// </summary>
    internal const char CloseParentheses = ')';
}
