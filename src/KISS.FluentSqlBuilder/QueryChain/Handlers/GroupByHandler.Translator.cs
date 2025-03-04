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
        switch (memberExpression.Expression)
        {
            // Accessing a property or field of a parameter in a lambda
            // case ParameterExpression parameterExpression:
            //     {
            //         (string alias, _) = Composite.MapProfiles[parameterExpression.Type];
            //         string key = memberExpression.Member.Name;
            //         string groupKey = $"{alias}_{memberExpression.Member.Name}";
            //         Composite.GroupKeys.Add((alias, key, groupKey));
            //         var propertyAssignment = Composite.ChangeType(Expression.Property(Composite.DapperRowVariable, "Item", Expression.Constant(groupKey)), memberExpression.Type);
            //         Composite.GroupingPropertyAssignmentProcessing.Add((memberExpression.Type, propertyAssignment));
            //         break;
            //     }

            case null:
            default:
                throw new NotSupportedException("Expression not supported.");
        }
    }

    /// <inheritdoc />
    protected override void Translate(UnaryExpression unaryExpression)
    {
        switch (unaryExpression.Operand)
        {
            // Accessing a property or field of a parameter in a lambda
            // case MemberExpression memberExpression:
            //     {
            //         (string alias, _) = Composite.MapProfiles[memberExpression.Member.DeclaringType!];
            //         string key = memberExpression.Member.Name;
            //         string groupKey = $"{alias}_{memberExpression.Member.Name}";
            //         Composite.GroupKeys.Add((alias, key, groupKey));
            //         break;
            //     }

            default:
                throw new NotSupportedException("Expression not supported.");
        }
    }

    /// <inheritdoc />
    protected override void Translate(ConstantExpression constantExpression)
    {
        Append($"{constantExpression.Value}");
    }
}