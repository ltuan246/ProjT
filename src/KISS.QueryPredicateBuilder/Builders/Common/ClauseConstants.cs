namespace KISS.QueryPredicateBuilder.Builders.Common;

/// <summary>
///     The Clause constants which containing frequently reused variables.
/// </summary>
internal static class ClauseConstants
{
    // private const char Comma = ',';

    /// <summary>
    ///     Used to begin parenthetical text.
    /// </summary>
    internal const char OpenParentheses = '(';

    /// <summary>
    ///     Denotes the end of parenthetical text.
    /// </summary>
    internal const char CloseParentheses = ')';

    // internal static class Select
    // {
    //     internal const string Clause = "SELECT";
    //     internal const string Distinct = "SELECT DISTINCT";
    //     internal const string From = "FROM";
    //     internal const char Separator = Comma;
    // }

    /// <summary>
    ///     A WHERE constant which containing frequently reused variables.
    /// </summary>
    internal static class Where
    {
        // internal const string Clause = "WHERE";

        /// <summary>
        ///     The OR operator.
        /// </summary>
        internal const string OrSeparator = " OR ";

        /// <summary>
        ///     The AND operator.
        /// </summary>
        internal const string AndSeparator = " AND ";
    }
}
