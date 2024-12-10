namespace KISS.FluentSqlBuilder.Visitors.QueryComponent.Components;

/// <summary>
///     A builder for a <c>ORDER BY</c> clause.
/// </summary>
/// <param name="Composite">The structure of the fluent SQL builder.</param>
public sealed record OrderByTranslator(CompositeQuery Composite) : ExpressionTranslator
{
    /// <inheritdoc />
    protected override void Translate(MemberExpression memberExpression)
    {
        switch (memberExpression.Expression)
        {
            // Accessing a property or field of a parameter in a lambda
            case ParameterExpression parameterExpression:
                {
                    Composite.Append(
                        $"{Composite.GetAliasMapping(parameterExpression.Type)}.{memberExpression.Member.Name}");
                    break;
                }
        }
    }
}
