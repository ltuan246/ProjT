namespace KISS.QueryBuilder.Core;

/// <summary>
///     A class that defines the fluent SQL builder type.
///     The core <see cref="QueryVisitor" /> partial class.
/// </summary>
internal sealed partial class QueryVisitor
{
    /// <summary>
    ///     Gets the generated the SQL.
    /// </summary>
    public string Sql
        => SqlBuilder.ToString();

    private StringBuilder SqlBuilder { get; } = new();
}
