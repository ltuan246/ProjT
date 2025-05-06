namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public sealed partial class CompositeQuery
{
    private static Type InEntryType { get; } = typeof(IDictionary<string, object>);

    /// <summary>
    /// CurrentEntryExParameter.
    /// </summary>
    public ParameterExpression CurrentEntryExParameter { get; } =
        Expression.Parameter(InEntryType, "CurrentEntryExParameter");
}
