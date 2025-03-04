namespace KISS.FluentSqlBuilder.QueryChain.Handlers;

/// <summary>
///     SelectHandler.
/// </summary>
/// <typeparam name="TSource">The type representing the database record set.</typeparam>
/// <typeparam name="TReturn">The combined type to return.</typeparam>
public sealed partial record NewSelectHandler<TSource, TReturn>
{
    /// <inheritdoc />
    protected override void Translate(MemberExpression memberExpression)
    {
        switch (memberExpression.Expression)
        {
            // Accessing a static member (constant or static field)
            case null:
                {
                    switch (memberExpression.Member)
                    {
                        // Accessing a static field, get its type and value using reflection
                        case FieldInfo fieldInfo:
                            var fieldType = fieldInfo.GetType();
                            AppendFormat($"{fieldInfo.GetValue(fieldType)}");
                            break;
                        // Accessing a static property, get its type and value using reflection
                        case PropertyInfo propertyInfo:
                            var propType = propertyInfo.GetType();
                            AppendFormat($"{propertyInfo.GetValue(propType)}");
                            break;
                    }

                    break;
                }

            // Accessing a property or field of a parameter in a lambda
            case ParameterExpression parameterExpression:
                {
                    Append($"{Composite.GetAliasMapping(parameterExpression.Type)}.{memberExpression.Member.Name}");

                    // if (UseAlias)
                    // {
                    //     string alias = $"{parameterExpression.Type.Name}{memberExpression.Member.Name}";
                    //     Composite.ColumnAliases.Add(alias);
                    //     Append($" AS {alias}");
                    // }

                    break;
                }

            // Accessing a field/property of a constant object
            case ConstantExpression constantExpression:
                {
                    var (evaluated, value) = Composite.GetValue(memberExpression);
                    if (evaluated)
                    {
                        AppendFormat(value);
                    }
                    else
                    {
                        Translate(constantExpression);
                    }

                    break;
                }

            // Accessing an object creation, method invocation, or method call (e.g., new Object().Property, obj.Method().Property)
            case NewExpression:
            case InvocationExpression:
            case MethodCallExpression:
                {
                    // Accessing the property or field (memberExpression.Member.Name) on the object or result of the method (memberExpression.Expression).
                    var member = Expression.Property(memberExpression.Expression, memberExpression.Member.Name);
                    var value = Expression.Lambda(member).Compile().DynamicInvoke();
                    AppendFormat($"{value}");
                    break;
                }

            default:
                {
                    Translate(memberExpression.Expression);
                    break;
                }
        }
    }

    /// <inheritdoc />
    protected override void Translate(NewExpression newExpression)
    {
        var selectList = newExpression.Members!
            .Select(m => m.Name)
            .Zip(newExpression.Arguments, (name, arg) =>
            {
                if (arg is MemberExpression memberExpression)
                {
                    string tableAlias = Composite.GetAliasMapping(memberExpression.Member.DeclaringType!);
                    return $"{tableAlias}." + (name == memberExpression.Member.Name
                        ? memberExpression.Member.Name
                        : $"{memberExpression.Member.Name} AS {name}");
                }

                return name;
            })
            .ToArray();

        Append(string.Join(", ", selectList));
    }

    /// <inheritdoc />
    protected override void Translate(MemberInitExpression memberInitExpression)
    {
        using var enumerator = memberInitExpression.Bindings.GetEnumerator();
        if (enumerator.MoveNext())
        {
            if (enumerator.Current is MemberAssignment
                { Expression: MemberExpression { Expression: ParameterExpression parameter1 } member1 } assignment1)
            {
                string alias = Composite.GetAliasMapping(parameter1.Type);
                string sourceMemberName = $"{alias}.{member1.Member.Name}";
                Append($"{sourceMemberName} AS {assignment1.Member.Name}");
            }

            while (enumerator.MoveNext())
            {
                if (enumerator.Current is MemberAssignment
                    { Expression: MemberExpression { Expression: ParameterExpression parameter2 } member2 } assignment2)
                {
                    Append(", ");
                    AppendLine(string.Empty, true);

                    string alias = Composite.GetAliasMapping(parameter2.Type);
                    string sourceMemberName = $"{alias}.{member2.Member.Name}";
                    Append($"{sourceMemberName} AS {assignment2.Member.Name}");
                }
            }
        }
    }

    /// <inheritdoc />
    protected override void Translate(UnaryExpression unaryExpression)
    {
        if (unaryExpression is { Operand: MemberExpression memberExpression })
        {
            Append($"{Composite.GetAliasMapping(memberExpression.Member.DeclaringType!)}.{memberExpression.Member.Name}");
        }
        else
        {
            throw new NotSupportedException("Expression not supported.");
        }
    }

    /// <inheritdoc />
    protected override void Translate(ConstantExpression constantExpression)
    {
        Append($"{constantExpression.Value}");
    }
}