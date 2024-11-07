namespace KISS.QueryBuilder.Visitors.QueryComponents;

/// <summary>
///     A builder for a <c>SELECT</c> clause.
/// </summary>
/// <param name="SqlFormat">Use to custom string formatting for SQL queries.</param>
/// <param name="TableAliases">A collection specifically for table aliases.</param>
/// <param name="HasDistinct">Use checks to know when to use <c>SELECT DISTINCT</c> clause.</param>
internal sealed record SelectComponent(
    SqlFormatter SqlFormat,
    Dictionary<Type, string> TableAliases,
    bool HasDistinct = false)
    : QueryComponent, IQueryComponent
{
    /// <summary>
    ///     The table columns.
    /// </summary>
    public List<Expression> Selectors { get; } = [];

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        Append(HasDistinct ? "SELECT" : "SELECT DISTINCT");
        AppendLine(true);

        foreach (var selector in Selectors)
        {
            Translate(selector);
        }

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
                    _ = constantExpression;
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
}
