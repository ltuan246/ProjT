namespace KISS.FluentSqlBuilder.Decorators.LimitDecorators;

/// <summary>
///     Implements a decorator for SQL LIMIT (or TOP) operations, enabling queries to restrict
///     the number of returned rows. This class extends the query builder to support result
///     limiting in composite SQL queries.
/// </summary>
public sealed partial record LimitDecorator(IComposite Inner) : QueryDecorator(Inner);
