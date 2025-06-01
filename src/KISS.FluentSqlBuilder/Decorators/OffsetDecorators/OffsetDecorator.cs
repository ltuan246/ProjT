namespace KISS.FluentSqlBuilder.Decorators.OffsetDecorators;

/// <summary>
///     Implements a decorator for SQL OFFSET operations, enabling queries to skip a specified
///     number of rows before returning results. This class extends the query builder to support
///     result offsetting in composite SQL queries.
/// </summary>
public sealed partial record OffsetDecorator(IComposite Inner) : QueryDecorator(Inner);
