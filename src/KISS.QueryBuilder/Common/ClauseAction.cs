namespace KISS.QueryBuilder.Common;

/// <summary>
///     Declares enum for the <see cref="FluentSqlBuilder{TRecordset}" /> type.
/// </summary>
internal enum ClauseAction
{
    Select,
    SelectFrom,
    Join,
    Where,
    GroupBy,
    Having,
    OrderBy,
    Limit,
    Offset
}
