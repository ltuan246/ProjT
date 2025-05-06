namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
public sealed partial class CompositeQuery
{
    /// <summary>
    ///     Creates a collection of <see cref="MemberBinding" /> instances by mapping properties
    ///     from an iterated row (e.g., a dictionary or dynamic object) to a target type.
    /// </summary>
    /// <param name="iterRowVariable">The current row being processed in the loop.</param>
    /// <param name="sourceType">The type of the source entity providing the data.</param>
    /// <param name="targetType">The type of the target entity to which properties are bound.</param>
    /// <returns>
    ///     An enumerable collection of <see cref="MemberBinding" /> objects,
    ///     where each binding represents the assignment of a source property value
    ///     to a corresponding target property.
    /// </returns>
    public IEnumerable<MemberBinding> CreateIterRowBindings(
        ParameterExpression iterRowVariable,
        Type sourceType,
        Type targetType)
    {
        var alias = GetAliasMapping(sourceType);
        var sourceProperties = sourceType.GetProperties().Where(p => p.CanWrite).ToList();
        foreach (var sourceProperty in sourceProperties)
        {
            var targetProperty = targetType.GetProperty(sourceProperty.Name);
            if (targetProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
            {
                var sourceValue = Expression.Property(
                    iterRowVariable,
                    "Item",
                    Expression.Constant($"{alias}_{sourceProperty.Name}"));

                yield return Expression.Bind(targetProperty, ChangeType(sourceValue, targetProperty.PropertyType));
            }
        }
    }

    /// <summary>
    /// Creates a block expression that initializes the target variable with a new instance
    /// of the specified output entity type, using member bindings generated from the source parameter.
    /// </summary>
    /// <param name="targetVariable">The variable that will be assigned the newly initialized object.</param>
    /// <param name="sourceParameter">The parameter expression representing the input entity source.</param>
    /// <param name="inEntityType">The type of the input entity from which member bindings will be created.</param>
    /// <param name="outEntityType">The type of the output entity to instantiate and initialize.</param>
    /// <returns>
    /// A <see cref="BlockExpression"/> that assigns a newly constructed and member-initialized
    /// <paramref name="outEntityType"/> object to <paramref name="targetVariable"/>.
    /// </returns>
    public BlockExpression InitializeTargetValueBlock(ParameterExpression targetVariable, ParameterExpression sourceParameter, Type inEntityType, Type outEntityType)
        => Expression.Block(
            Expression.Assign(
                targetVariable,
                Expression.MemberInit(
                    Expression.New(outEntityType),
                    CreateIterRowBindings(sourceParameter, inEntityType, outEntityType))));

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
    private Expression ChangeType(IndexExpression sourceValue, Type targetType)
    {
        // If the target type is nullable (e.g., Nullable<T>), retrieve its underlying non-nullable type (T).
        // This is necessary because Expression.Convert cannot directly convert a non-nullable value to a nullable type.
        // By first converting to the underlying type, we ensure compatibility before handling the nullable conversion.
        var nonNullableType = Nullable.GetUnderlyingType(targetType);
        var effectiveTargetType = nonNullableType ?? targetType;

        if (effectiveTargetType == typeof(Guid)
            || effectiveTargetType == typeof(DateTime)
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

    private Expression ChangeSpecificType(IndexExpression sourceValue, Type targetType)
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
            Expression.Constant(null, typeof(string)),
            Expression.Call(
                Expression.Convert(sourceValue, typeof(object)),
                typeof(object).GetMethod("ToString", Type.EmptyTypes)!));

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
}
