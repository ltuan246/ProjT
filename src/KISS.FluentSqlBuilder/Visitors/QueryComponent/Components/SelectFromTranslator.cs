namespace KISS.FluentSqlBuilder.Visitors.QueryComponent.Components;

/// <summary>
///     A builder for a <c>FROM</c> clause.
/// </summary>
/// <param name="Composite">The structure of the fluent SQL builder.</param>
public sealed record SelectFromTranslator(CompositeQuery Composite) : ExpressionTranslator
{
    /// <inheritdoc />
    protected override void Translate(ConstantExpression constantExpression)
    {
        Composite.Append(
            $"{((Type)constantExpression.Value!).Name}s {Composite.GetAliasMapping((Type)constantExpression.Value)}");
    }
}
