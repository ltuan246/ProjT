namespace KISS.QueryPredicateBuilder.Builders.Common;

internal static class ClauseConstants
{
    private const char Comma = ',';

    internal const char OpenParentheses = '(';
    internal const char CloseParentheses = ')';

    internal static class Select
    {
        internal const string Clause = "SELECT";
        internal const string Distinct = "SELECT DISTINCT";
        internal const string From = "FROM";
        internal const char Separator = Comma;
    }

    internal static class Where
    {
        internal const string Clause = "WHERE";
        internal const string OrSeparator = " OR ";
        internal const string AndSeparator = " AND ";
    }
}