namespace KISS.FluentSqlBuilder.Decorators.GroupByDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public sealed partial record GroupByDecorator
{
    /// <summary>
    ///     OutputVariable.
    /// </summary>
    public ParameterExpression OutputVariable { get; init; }

    /// <summary>
    ///     OutputVariable.
    /// </summary>
    public ParameterExpression OuterDictObjEntityVariable { get; init; }

    /// <summary>
    ///     OutputVariable.
    /// </summary>
    public ParameterExpression OuterKeyVariable { get; init; }

    /// <summary>
    ///     OutputVariable.
    /// </summary>
    public ParameterExpression InnerKeyVariable { get; init; }

    /// <summary>
    ///     OutputVariable.
    /// </summary>
    public ParameterExpression OuterDictIterVariable { get; init; }

    /// <summary>
    ///     OutputVariable.
    /// </summary>
    public ParameterExpression OuterDictEntryParameter { get; init; }
}
