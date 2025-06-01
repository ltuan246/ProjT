namespace KISS.FluentSqlBuilder.Decorators.JoinDecorators;

/// <summary>
///     Implements a decorator for SQL JOIN operations, enabling the composition of queries
///     that join multiple tables or entities. This class extends the query builder to support
///     join logic and type-safe result processing in composite SQL queries.
/// </summary>
public sealed partial record JoinDecorator(IComposite Inner) : QueryDecorator(Inner);
