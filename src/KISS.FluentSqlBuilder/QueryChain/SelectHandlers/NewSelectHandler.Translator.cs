namespace KISS.FluentSqlBuilder.QueryChain.SelectHandlers;

/// <summary>
///     A handler for processing SELECT clauses with object initialization in a query chain.
///     This class provides the translation logic for converting object initialization expressions
///     into SQL-compatible form.
/// </summary>
/// <typeparam name="TSource">
///     The type representing the database record set.
///     This type defines the structure of the data being queried.
/// </typeparam>
/// <typeparam name="TReturn">
///     The type of the object to be created and returned.
///     This type defines the structure of the result object.
/// </typeparam>
public sealed partial record NewSelectHandler<TSource, TReturn>
{
    /// <summary>
    ///     Translates a member expression into SQL for object initialization.
    ///     Handles various scenarios for accessing members of objects, including static members,
    ///     parameter members, constant members, and method call results.
    /// </summary>
    /// <param name="memberExpression">The member expression to translate.</param>
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

    /// <summary>
    ///     Translates a new expression into SQL for object initialization.
    ///     Handles the creation of new objects with property assignments.
    /// </summary>
    /// <param name="newExpression">The new expression to translate.</param>
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

    /// <summary>
    ///     Translates a member initialization expression into SQL.
    ///     Handles the initialization of object properties with values from the source.
    /// </summary>
    /// <param name="memberInitExpression">The member initialization expression to translate.</param>
    protected override void Translate(MemberInitExpression memberInitExpression)
    {
        new EnumeratorProcessor<MemberBinding>(memberInitExpression.Bindings)
            .AccessFirst(m =>
            {
                // if (m is MemberAssignment
                //     { Expression: MemberExpression { Expression: ParameterExpression parameter } member } assignment)
                // {
                //     string alias = Composite.GetAliasMapping(parameter.Type);
                //     string sourceMemberName = $"{alias}.{member.Member.Name}";
                //     Append($"{sourceMemberName} AS {assignment.Member.Name}");
                // }

                if (m is MemberAssignment
                    { Expression: MemberExpression { Expression: ParameterExpression parameter } member } assignment)
                {
                }
            })
            .AccessRemaining(m =>
            {
                if (m is MemberAssignment
                    { Expression: MemberExpression { Expression: ParameterExpression parameter } member } assignment)
                {
                    Append(", ");
                    AppendLine(string.Empty, true);

                    string alias = Composite.GetAliasMapping(parameter.Type);
                    string sourceMemberName = $"{alias}.{member.Member.Name}";
                    Append($"{sourceMemberName} AS {assignment.Member.Name}");
                }
            })
            .Execute();
    }

    /// <summary>
    ///     Translates a unary expression into SQL for object initialization.
    ///     Handles operations like negation and type conversion.
    /// </summary>
    /// <param name="unaryExpression">The unary expression to translate.</param>
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

    /// <summary>
    ///     Translates a constant expression into SQL for object initialization.
    ///     Converts constant values into their string representation for SQL.
    /// </summary>
    /// <param name="constantExpression">The constant expression to translate.</param>
    protected override void Translate(ConstantExpression constantExpression)
    {
        Append($"{constantExpression.Value}");
    }
}
