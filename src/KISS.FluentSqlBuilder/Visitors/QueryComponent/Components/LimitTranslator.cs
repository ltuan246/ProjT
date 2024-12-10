namespace KISS.FluentSqlBuilder.Visitors.QueryComponent.Components;

/// <summary>
///     A builder for a <c>LIMIT</c> clause.
/// </summary>
/// <param name="Composite">The structure of the fluent SQL builder.</param>
public sealed record LimitTranslator(CompositeQuery Composite) : ExpressionTranslator
{
    /// <inheritdoc />
    protected override void Translate(ConstantExpression constantExpression)
    {
        Composite.Append($"{constantExpression.Value}");
    }
}
