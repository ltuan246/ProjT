namespace KISS.FluentSqlBuilder.QueryChain.Handlers;

/// <summary>
///     GroupByHandler.
/// </summary>
/// <param name="Selector">Selector.</param>
public sealed partial record GroupByHandler
{
    /// <inheritdoc />
    protected override void Translate(MemberExpression memberExpression)
    {
        if (memberExpression is { Expression: ParameterExpression parameterExpression })
        {
            string alias = Composite.GetAliasMapping(parameterExpression.Type);
            string fieldName = $"{alias}_{memberExpression.Member.Name}";
            Composite.GroupingKeys[fieldName] = memberExpression.Type;
            Append($"{fieldName}");
        }
        else
        {
            throw new NotSupportedException("Expression not supported.");
        }
    }

    /// <inheritdoc />
    protected override void Translate(UnaryExpression unaryExpression)
    {
        if (unaryExpression is { Operand: MemberExpression memberExpression })
        {
            string alias = Composite.GetAliasMapping(memberExpression.Member.DeclaringType!);
            string fieldName = $"{alias}_{memberExpression.Member.Name}";
            Composite.GroupingKeys[fieldName] = memberExpression.Type;
            Append($"{fieldName}");
        }
        else
        {
            throw new NotSupportedException("Expression not supported.");
        }
    }
}