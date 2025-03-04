namespace KISS.FluentSqlBuilder.QueryChain.Handlers;

/// <summary>
///     OrderByHandler.
/// </summary>
/// <param name="Selector">Selector.</param>
public sealed partial record OrderByHandler
{
    /// <inheritdoc />
    protected override void Translate(MemberExpression memberExpression)
    {
        if (memberExpression is { Expression: ParameterExpression parameterExpression })
        {
            Append($"{Composite.GetAliasMapping(parameterExpression.Type)}.{memberExpression.Member.Name}");
        }
        else
        {
            throw new NotSupportedException("Expression not supported.");
        }
    }
}
