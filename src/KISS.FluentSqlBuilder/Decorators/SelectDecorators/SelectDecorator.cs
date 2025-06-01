namespace KISS.FluentSqlBuilder.Decorators.SelectDecorators;

/// <summary>
///     Implements a decorator for SQL SELECT operations, enabling the construction and execution
///     of SQL queries with type-safe result mapping. This class extends the query builder to support
///     flexible SELECT clause generation in composite SQL queries.
/// </summary>
public sealed partial record SelectDecorator(IComposite Inner) : QueryDecorator(Inner);
