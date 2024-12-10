namespace KISS.FluentSqlBuilder.Visitors.QueryComponent.Components;

/// <summary>
///     A builder for a <c>HAVING</c> clause.
/// </summary>
/// <param name="Composite">The structure of the fluent SQL builder.</param>
public sealed record HavingTranslator(CompositeQuery Composite) : ExpressionTranslator;
