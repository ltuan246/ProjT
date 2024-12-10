namespace KISS.FluentSqlBuilder.Visitors.QueryComponent.Components;

/// <summary>
///     A builder for a <c>GROUP BY</c> clause, grouping records by a specified key.
/// </summary>
/// <param name="Composite">The structure of the fluent SQL builder.</param>
public sealed record GroupByTranslator(CompositeQuery Composite) : ExpressionTranslator;
