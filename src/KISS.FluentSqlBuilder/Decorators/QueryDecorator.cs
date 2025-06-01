namespace KISS.FluentSqlBuilder.Decorators;

/// <summary>
///     Abstract base class for SQL query decorators, providing a foundation for building and executing
///     composite SQL queries. This class wraps an <see cref="IComposite"/> instance and exposes
///     core properties for entity types, output collections, and table alias management. It enables
///     extension through partial records for specialized query behaviors (e.g., joins, grouping, limits).
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
