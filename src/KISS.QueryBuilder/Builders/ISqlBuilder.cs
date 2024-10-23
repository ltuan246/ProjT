namespace KISS.QueryBuilder.Builders;

/// <summary>
///     An interface that defines the SQL builder type.
/// </summary>
public interface ISqlBuilder
{
    /// <summary>
    ///     Gets the generated the SQL.
    /// </summary>
    string Sql { get; }
}