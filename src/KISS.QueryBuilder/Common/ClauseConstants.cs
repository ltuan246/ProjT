namespace KISS.QueryBuilder.Common;

/// <summary>
///     Declares const for the <see cref="FluentSqlBuilder{TRecordset}" /> type.
/// </summary>
internal static class ClauseConstants
{
    /// <summary>
    ///     Used to address <c>TRecordset</c>.
    /// </summary>
    internal const string DefaultTableAlias = "Extend";

    /// <summary>
    ///     Comma-separated string.
    /// </summary>
    internal const char Comma = ',';

    /// <summary>
    ///     Used to begin parenthetical text.
    /// </summary>
    internal const char OpenParenthesis = '(';

    /// <summary>
    ///     Used to end parenthetical text.
    /// </summary>
    internal const char CloseParenthesis = ')';

    /// <summary>
    ///     The <c>SELECT</c> text.
    /// </summary>
    internal const string Select = "SELECT";

    /// <summary>
    ///     The <c>DISTINCT</c> text.
    /// </summary>
    internal const string Distinct = "DISTINCT";

    /// <summary>
    ///     The <c>FROM</c> text.
    /// </summary>
    internal const string From = "FROM";

    /// <summary>
    ///     The <c>JOIN</c> text.
    /// </summary>
    internal const string Join = "JOIN";

    /// <summary>
    ///     The <c>ON</c> text.
    /// </summary>
    internal const string OnSeparator = "ON";
}
