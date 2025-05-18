namespace KISS.FluentSqlBuilder.Utils;

/// <summary>
/// TypeUtils.
/// </summary>
public sealed record TypeUtils
{
    /// <summary>
    /// IteratorType.
    /// </summary>
    private static Type IteratorType { get; } = typeof(IEnumerator);

    /// <summary>
    /// DisposableType.
    /// </summary>
    private static Type DisposableType { get; } = typeof(IDisposable);

    /// <summary>
    /// DapperRowType.
    /// </summary>
    public static Type DapperRowType { get; } = typeof(IDictionary<string, object>);

    /// <summary>
    /// DapperRowCollectionType.
    /// </summary>
    public static Type DapperRowCollectionType { get; } = typeof(IEnumerable<IDictionary<string, object>>);

    /// <summary>
    /// DapperRowCollectionType.
    /// </summary>
    public static Type DapperRowIteratorType { get; } = typeof(IEnumerator<IDictionary<string, object>>);

    /// <summary>
    /// ObjType.
    /// </summary>
    public static Type ObjType { get; } = typeof(object);

    /// <summary>
    /// ObjType.
    /// </summary>
    public static Type StrType { get; } = typeof(string);

    /// <summary>
    /// ObjType.
    /// </summary>
    public static Type GuidType { get; } = typeof(Guid);

    /// <summary>
    /// ObjType.
    /// </summary>
    public static Type DateTimeType { get; } = typeof(DateTime);

    /// <summary>
    /// ObjType.
    /// </summary>
    public static Type ValueTupleType { get; } = typeof(ValueTuple);

    /// <summary>
    /// ObjType.
    /// </summary>
    public static Type DictionaryType { get; } = typeof(Dictionary<,>);

    /// <summary>
    /// IterMoveNextMethod.
    /// </summary>
    public static MethodInfo IterMoveNextMethod { get; } = IteratorType.GetMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes)!;

    /// <summary>
    /// DisposeMethod.
    /// </summary>
    public static MethodInfo DisposeMethod { get; } = DisposableType.GetMethod(nameof(IDisposable.Dispose), Type.EmptyTypes)!;

    /// <summary>
    ///     Gets the MethodInfo for the GetEnumerator method of <see cref="IEnumerable{IDictionary}" />.
    ///     This is used to create an enumerator for iterating over collections of dictionaries in expression trees.
    /// </summary>
    /// <remarks>
    ///     Cached as a static property to avoid repeated reflection calls,
    ///     improving performance in expression tree construction.
    ///     The specific type
    ///     <see cref="IEnumerable{IDictionary}" /> ensures compatibility with dictionary-based data rows.
    /// </remarks>
    public static MethodInfo GetEnumeratorForIEnumDictStrObj { get; } = DapperRowCollectionType.GetMethod("GetEnumerator")!;

    /// <summary>
    ///     Converts an <see cref="IndexExpression"/> value, typically sourced from a dictionary-like structure,
    ///     to a specified target type, handling both nullable and non-nullable conversions.
    /// </summary>
    /// <param name="sourceValue">
    ///     The source value as an <see cref="IndexExpression"/>,
    ///     assumed to originate from an <see cref="IDictionary{TKey, TValue}"/>.
    ///     This value may represent a string, null, or other type requiring conversion.
    /// </param>
    /// <param name="targetType">
    ///     The desired target type for conversion, which may be nullable
    ///     (e.g., <see cref="Nullable{T}"/>) or non-nullable (e.g., <see cref="Guid"/> or <see cref="int"/>).
    /// </param>
    /// <returns>
    ///     A <see cref="Expression"/> representing the converted value, adjusted to match the
    ///     <paramref name="targetType"/>. For nullable types, includes null checks to handle null source values
    ///     appropriately.
    /// </returns>
    public static Expression ChangeType(IndexExpression sourceValue, Type targetType)
    {
        // If the target type is nullable (e.g., Nullable<T>), retrieve its underlying non-nullable type (T).
        // This is necessary because Expression.Convert cannot directly convert a non-nullable value to a nullable type.
        // By first converting to the underlying type, we ensure compatibility before handling the nullable conversion.
        var nonNullableType = Nullable.GetUnderlyingType(targetType);
        var effectiveTargetType = nonNullableType ?? targetType;

        if (effectiveTargetType == GuidType
            || effectiveTargetType == DateTimeType
            || nonNullableType is not null)
        {
            return ChangeSpecificType(sourceValue, targetType);
        }

        var changeTypeCall = Expression.Call(
            typeof(Convert),
            nameof(Convert.ChangeType),
            Type.EmptyTypes,
            sourceValue,
            Expression.Constant(targetType));

        var convertedValue = Expression.ConvertChecked(changeTypeCall, targetType);

        return convertedValue;
    }

    /// <summary>
    ///     Converts an <see cref="IndexExpression"/> value, typically sourced from a dictionary-like structure,
    ///     to a specified target type, handling both nullable and non-nullable conversions.
    /// </summary>
    /// <param name="sourceValue">
    ///     The source value as an <see cref="IndexExpression"/>,
    ///     assumed to originate from an <see cref="IDictionary{TKey, TValue}"/>.
    ///     This value may represent a string, null, or other type requiring conversion.
    /// </param>
    /// <param name="targetType">
    ///     The desired target type for conversion, which may be nullable
    ///     (e.g., <see cref="Nullable{T}"/>) or non-nullable (e.g., <see cref="Guid"/> or <see cref="int"/>).
    /// </param>
    /// <returns>
    ///     A <see cref="Expression"/> representing the converted value, adjusted to match the
    ///     <paramref name="targetType"/>. For nullable types, includes null checks to handle null source values
    ///     appropriately.
    /// </returns>
    private static Expression ChangeSpecificType(IndexExpression sourceValue, Type targetType)
    {
        // If the target type is nullable (e.g., Nullable<T>), retrieve its underlying non-nullable type (T).
        // This is necessary because Expression.Convert cannot directly convert a non-nullable value to a nullable type.
        // By first converting to the underlying type, we ensure compatibility before handling the nullable conversion.
        var nonNullableType = Nullable.GetUnderlyingType(targetType);
        Type actualType = nonNullableType ?? targetType;

        // Check if sourceValue is null
        var isSourceNullCheck = Expression.Equal(sourceValue, Expression.Constant(null));

        // Convert sourceValue to string only if non-null (mirrors data?.ToString())
        Expression sourceString = Expression.Condition(
            isSourceNullCheck,
            Expression.Constant(null, StrType),
            CallMethod(Expression.Convert(sourceValue, ObjType), "ToString"));

        // TryParse logic
        var result = Expression.Variable(actualType, "result");
        var tryParse = Expression.Call(
            actualType,
            "TryParse",
            Type.EmptyTypes,
            sourceString,
            result);

        // Block to execute TryParse and return the result
        var parseResultBlock = Expression.Block([result], tryParse, result);

        return nonNullableType is null ? parseResultBlock : Expression.Convert(parseResultBlock, targetType);
    }

    /// <summary>
    /// Creates an expression representing a ContainsKey method call on a dictionary.
    /// </summary>
    /// <param name="dict">The expression representing the dictionary.</param>
    /// <param name="key">The expression representing the key to check in the dictionary.</param>
    /// <returns>An expression representing dict.ContainsKey(key).</returns>
    /// <exception cref="InvalidOperationException">Thrown if the dictionary type does not support ContainsKey for the key type.</exception>
    public static MethodCallExpression IsDictContainsKey(Expression dict, Expression key)
        => CallMethod(dict, "ContainsKey", key);

    /// <summary>
    /// Creates an expression that calls a specified method on a target instance with the given arguments.
    /// </summary>
    /// <param name="instance">The expression representing the instance (e.g., dictionary or list).</param>
    /// <param name="methodName">The name of the method to call (e.g., "Add").</param>
    /// <param name="arguments">The expressions representing the method arguments.</param>
    /// <returns>An expression representing the call to the specified method.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the method is not found on the instance type.</exception>
    public static MethodCallExpression CallMethod(Expression instance, string methodName, params Expression[] arguments)
    {
        var instanceType = instance.Type;
        var argumentTypes = arguments.Select(arg => arg.Type).ToArray();
        var method = instanceType.GetMethod(methodName, argumentTypes);
        return method == null
            ? throw new InvalidOperationException($"{methodName} method not found on type {instanceType}")
            : Expression.Call(instance, method, arguments);
    }

    /// <summary>
    /// Creates a block expression that initializes the target variable with a new instance
    /// of the specified output entity type, using member bindings generated from the source parameter.
    /// </summary>
    /// <param name="targetVariable">The variable that will be assigned the newly initialized object.</param>
    /// <param name="bindings">
    ///     An enumerable collection of <see cref="MemberBinding" /> objects,
    ///     where each binding represents the assignment of a source property value
    ///     to a corresponding target property.
    /// </param>
    /// <returns>
    /// A <see cref="BlockExpression"/> that assigns a newly constructed and member-initialized
    /// <paramref name="outEntityType"/> object to <paramref name="targetVariable"/>.
    /// </returns>
    public static BinaryExpression InitializeTargetValue(ParameterExpression targetVariable, IEnumerable<MemberBinding> bindings)
        => Expression.Assign(
            targetVariable,
            Expression.MemberInit(Expression.New(targetVariable.Type), bindings));

    /// <summary>
    /// Creates a block expression that initializes the target variable with a new instance
    /// of the specified output entity type, using member bindings generated from the source parameter.
    /// </summary>
    /// <param name="targetVariable">The variable that will be assigned the newly initialized object.</param>
    /// <returns>
    /// A <see cref="BlockExpression"/> that assigns a newly constructed and member-initialized
    /// <paramref name="outEntityType"/> object to <paramref name="targetVariable"/>.
    /// </returns>
    public static BinaryExpression InitializeTargetValue(ParameterExpression targetVariable)
        => Expression.Assign(targetVariable, Expression.New(targetVariable.Type));

    /// <summary>
    ///     Creates a collection of <see cref="MemberBinding" /> instances by mapping properties
    ///     from an iterated row (e.g., a dictionary or dynamic object) to a target type.
    /// </summary>
    /// <param name="iterRowVariable">The current row being processed in the loop.</param>
    /// <param name="sourceType">The type of the source entity providing the data.</param>
    /// <param name="targetType">The type of the target entity to which properties are bound.</param>
    /// <param name="aliasName">A table alias for the source type.</param>
    /// <returns>
    ///     An enumerable collection of <see cref="MemberBinding" /> objects,
    ///     where each binding represents the assignment of a source property value
    ///     to a corresponding target property.
    /// </returns>
    public static IEnumerable<MemberBinding> CreateIterRowBindings(
        ParameterExpression iterRowVariable,
        Type sourceType,
        Type targetType,
        string aliasName)
    {
        var sourceProperties = sourceType.GetProperties().Where(p => p.CanWrite).ToList();
        foreach (var sourceProperty in sourceProperties)
        {
            var targetProperty = targetType.GetProperty(sourceProperty.Name);
            if (targetProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
            {
                var sourceValue = Expression.Property(
                    iterRowVariable,
                    "Item",
                    Expression.Constant($"{aliasName}_{sourceProperty.Name}"));

                yield return Expression.Bind(targetProperty, ChangeType(sourceValue, targetProperty.PropertyType));
            }
        }
    }

    /// <summary>
    ///     Retrieves the database table name associated with a given type using the SqlTableAttribute.
    /// </summary>
    /// <param name="type">The type (class) for which to retrieve the table name.</param>
    /// <returns>The name of the table as specified by the SqlTableAttribute on the type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the specified type does not have a SqlTableAttribute applied.</exception>
    public static string GetTableName(Type type)
    {
        var attr = type.GetCustomAttribute<SqlTableAttribute>();
        return attr == null
            ? throw new InvalidOperationException($"Type {type.Name} must have a SqlTableAttribute.")
            : attr.Name;
    }
}
