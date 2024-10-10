namespace KISS.QueryBuilder.FluentBuilder;

/// <summary>
///     An interface that defines the SQL builder type.
/// </summary>
internal interface ISqlBuilder
{
    /// <summary>
    ///     Gets the generated the SQL.
    /// </summary>
    string Sql { get; }
}
