namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     A handler for processing <c>SELECT</c> in a query chain.
/// </summary>
public sealed partial record SelectAggregateHandler
{
    /// <inheritdoc />
    protected override void Translate(MemberExpression memberExpression)
    {
        if (memberExpression is { Expression: ParameterExpression parameterExpression })
        {
            Append($"{Composite.GetAliasMapping(parameterExpression.Type)}_{memberExpression.Member.Name}");
            switch (memberExpression.Member)
            {
                // Accessing a static field, get its type and value using reflection
                case FieldInfo fieldInfo:
                    var fieldType = fieldInfo.FieldType;
                    Composite.AggregationKeys[Alias] = fieldType;
                    break;
                // Accessing a static property, get its type and value using reflection
                case PropertyInfo propertyInfo:
                    var propType = propertyInfo.PropertyType;
                    Composite.AggregationKeys[Alias] = propType;
                    break;
            }
        }
    }

    /// <inheritdoc />
    protected override void Translate(ConstantExpression constantExpression)
        => AppendFormat($"{constantExpression.Value}");

    /// <inheritdoc />
    protected override void Translate(BinaryExpression binaryExpression)
    {
        Translate(binaryExpression.Left);
        Append(BinaryOperandMap[binaryExpression.NodeType]);
        Translate(binaryExpression.Right);
    }

    /// <inheritdoc />
    protected override void Translate(UnaryExpression unaryExpression)
    {
        if (unaryExpression is { Operand: Expression expression })
        {
            Translate(expression);
        }
    }

    /// <inheritdoc />
    protected override void Translate(LambdaExpression lambdaExpression)
    {
        if (lambdaExpression is { Body: Expression expression })
        {
            Translate(expression);
        }
    }

    /// <inheritdoc />
    protected override void Translate(MethodCallExpression methodCallExpression)
    {
        if (methodCallExpression is { Arguments: [Expression expression] })
        {
            Append($"{methodCallExpression.Method.Name}(");
            Translate(expression);
            Append($") AS {Alias} ");
        }
    }
}