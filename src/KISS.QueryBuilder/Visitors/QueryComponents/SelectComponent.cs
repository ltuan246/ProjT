namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>SELECT</c> clause.
/// </summary>
/// <param name="sqlFormat">Use to custom string formatting for SQL queries.</param>
/// <param name="tableAliases">A collection specifically for table aliases.</param>
/// <param name="hasDistinct">Use checks to know when to use <c>SELECT DISTINCT</c> clause.</param>
internal sealed class SelectComponent(
    SqlFormatter sqlFormat,
    Dictionary<Type, string> tableAliases,
    bool hasDistinct = false) : QueryComponent
{
    /// <summary>
    ///     Use to custom string formatting for SQL queries.
    /// </summary>
    protected override SqlFormatter SqlFormat { get; } = sqlFormat;

    /// <summary>
    ///     A collection specifically for table aliases.
    /// </summary>
    protected override Dictionary<Type, string> TableAliases { get; } = tableAliases;

    /// <summary>
    ///     The table columns.
    /// </summary>
    public List<Expression> Selectors { get; } = [];

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        Append(hasDistinct ? "SELECT DISTINCT" : "SELECT");
        AppendLine(true);

        using var enumerator = Selectors.GetEnumerator();
        if (enumerator.MoveNext())
        {
            Translate(enumerator.Current);
            while (enumerator.MoveNext())
            {
                Append(", ");
                Translate(enumerator.Current);
            }
        }
        else
        {
            Append("*");
        }

        AppendLine();

        visitor.Visit(this);
    }

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
                    Append($"{GetAliasMapping(parameterExpression.Type)}.{memberExpression.Member.Name}");
                    break;
                }

            // Accessing a field/property of a constant object
            case ConstantExpression constantExpression:
                {
                    var (evaluated, value) = GetValue(memberExpression);
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
                    string tableAlias = GetAliasMapping(memberExpression.Member.DeclaringType!);
                    return $"{tableAlias}." + (name == memberExpression.Member.Name
                        ? memberExpression.Member.Name
                        : $"{memberExpression.Member.Name} AS {name}");
                }

                return name;
            })
            .ToArray();

        Append(string.Join(", ", selectList));
    }
}
