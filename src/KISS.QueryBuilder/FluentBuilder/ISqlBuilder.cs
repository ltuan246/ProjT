namespace KISS.QueryBuilder.FluentBuilder;

/// <summary>
///     An interface that defines the SQL builder type.
/// </summary>
public interface ISqlBuilder
{
    /// <summary>
    ///     Gets the generated the SQL.
    /// </summary>
    string Sql { get; }

    /// <summary>
    ///     A dynamic object that can be passed to the Query method instead of normal parameters.
    /// </summary>
    public DynamicParameters Parameters { get; }
}
