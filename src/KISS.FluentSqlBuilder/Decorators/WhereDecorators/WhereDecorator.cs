namespace KISS.FluentSqlBuilder.Decorators.WhereDecorators;

/// <summary>
///     Implements a decorator for SQL WHERE operations, enabling the construction and execution
///     of SQL queries with filtering conditions. This class extends the query builder to support
///     flexible WHERE clause generation in composite SQL queries.
/// </summary>
public sealed partial record WhereDecorator(IComposite Inner) : QueryDecorator(Inner);
