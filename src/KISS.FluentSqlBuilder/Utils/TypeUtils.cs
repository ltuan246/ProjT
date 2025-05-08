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
    private static Type DapperRowCollectionType { get; } = typeof(IEnumerable<IDictionary<string, object>>);

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
    /// IterMoveNextMethod.
    /// </summary>
    public static MethodInfo IterMoveNextMethod { get; } = IteratorType.GetMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes)!;

    /// <summary>
    /// DisposeMethod.
    /// </summary>
    public static MethodInfo DisposeMethod { get; } = DisposableType.GetMethod(nameof(IDisposable.Dispose), Type.EmptyTypes)!;

    /// <summary>
    /// ObjToStringMethod.
    /// </summary>
    public static MethodInfo ObjToStringMethod { get; } = ObjType.GetMethod(nameof(object.ToString), Type.EmptyTypes)!;

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
            Expression.Call(Expression.Convert(sourceValue, ObjType), ObjToStringMethod));

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
}
