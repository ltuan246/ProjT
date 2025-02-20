namespace KISS.FluentSqlBuilder.Visitors.QueryComponent.Components;

/// <summary>
///     A builder for a <c>OFFSET</c> clause.
/// </summary>
/// <param name="Composite">The structure of the fluent SQL builder.</param>
public sealed record OffsetTranslator(ICompositeQuery Composite) : ExpressionTranslator
{
    /// <inheritdoc />
    protected override void Translate(ConstantExpression constantExpression)
    {
        Composite.Append($"{constantExpression.Value}");
    }
}
