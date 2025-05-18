namespace KISS.FluentSqlBuilder.Decorators.OrderByDecorators;

/// <summary>
///     A sealed class that constructs and executes SQL queries using a database connection.
///     This class serves as the core component for building and executing composite SQL queries,
///     supporting both simple and complex query scenarios with type-safe result processing.
/// </summary>
public sealed partial record OrderByDecorator(IComposite Inner) : QueryDecorator(Inner);
