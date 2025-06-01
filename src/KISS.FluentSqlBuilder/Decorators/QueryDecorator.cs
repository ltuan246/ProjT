namespace KISS.FluentSqlBuilder.Decorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public abstract partial record QueryDecorator(IComposite Inner) : IComposite
{
    /// <inheritdoc />
    public Type InEntityType { get; } = Inner.InEntityType;

    /// <inheritdoc />
    public Type OutEntityType { get; } = Inner.OutEntityType;

    /// <inheritdoc />
    public Type OutEntitiesType { get; } = Inner.OutEntitiesType;

    /// <inheritdoc />
    public Dictionary<Type, string> TableAliases { get; } = Inner.TableAliases;
}
