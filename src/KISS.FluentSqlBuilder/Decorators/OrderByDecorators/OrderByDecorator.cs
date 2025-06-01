namespace KISS.FluentSqlBuilder.Decorators.OrderByDecorators;

/// <summary>
///     Implements a decorator for SQL ORDER BY operations, enabling the construction and execution
///     of SQL queries with sorting logic. This class extends the query builder to support
///     flexible ORDER BY clause generation in composite SQL queries.
/// </summary>
public sealed partial record OrderByDecorator(IComposite Inner) : QueryDecorator(Inner);
