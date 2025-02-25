namespace KISS.FluentSqlBuilder.QueryHandlerChain;

/// <summary>
///     CompositeQuery.
/// </summary>
public sealed partial class CompositeQuery
{
    /// <summary>
    /// Creates a collection of <see cref="MemberBinding"/> instances by mapping properties
    /// from an iterated row (e.g., a dictionary or dynamic object) to a target type.
    /// </summary>
    /// <param name="sourceType">The type of the source entity providing the data.</param>
    /// <param name="targetType">The type of the target entity to which properties are bound.</param>
    /// <returns>
    /// An enumerable collection of <see cref="MemberBinding"/> objects,
    /// where each binding represents the assignment of a source property value
    /// to a corresponding target property.
    /// </returns>
    private IEnumerable<MemberBinding> CreateIterRowBindings(Type sourceType, Type targetType)
    {
        var alias = GetAliasMapping(sourceType);
        var sourceProperties = sourceType.GetProperties().Where(p => p.CanWrite).ToList();
        foreach (var sourceProperty in sourceProperties)
        {
            var targetProperty = targetType.GetProperty(sourceProperty.Name);
            if (targetProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
            {
                var sourceValue = Expression.Property(IterationRowVariable, "Item", Expression.Constant($"{alias}_{sourceProperty.Name}"));

                // If the target type is nullable (e.g., Nullable<T>), retrieve its underlying non-nullable type (T).
                // This is necessary because Expression.Convert cannot directly convert a non-nullable value to a nullable type.
                // By first converting to the underlying type, we ensure compatibility before handling the nullable conversion.
                var nonNullableType = Nullable.GetUnderlyingType(targetProperty.PropertyType);

                // Nullable.GetUnderlyingType(typeof(int?))  // returns typeof(int)
                // Nullable.GetUnderlyingType(typeof(int))   // returns null
                if (nonNullableType is null)
                {
                    // Ensure proper conversion from object to the target type
                    var changeTypeCall = Expression.Call(
                        typeof(Convert),
                        nameof(Convert.ChangeType),
                        Type.EmptyTypes,
                        sourceValue,
                        Expression.Constant(targetProperty.PropertyType));

                    var convertedValue = Expression.ConvertChecked(changeTypeCall, targetProperty.PropertyType);

                    yield return Expression.Bind(targetProperty, convertedValue);
                }
                else
                {
                    var isNullCheck = Expression.Equal(sourceValue, Expression.Constant(null));

                    var defaultValue = Expression.Convert(
                        Expression.Call(typeof(Activator), nameof(Activator.CreateInstance), Type.EmptyTypes, Expression.Constant(nonNullableType)),
                        nonNullableType);

                    var changeTypeCall = Expression.Call(
                        typeof(Convert),
                        nameof(Convert.ChangeType),
                        Type.EmptyTypes,
                        sourceValue,
                        Expression.Constant(nonNullableType));

                    // Use Convert.ChangeType to dynamically convert the value
                    var conversion = Expression.ConvertChecked(
                        changeTypeCall,
                        nonNullableType);

                    // Use a conditional expression: if (value == null) default(T) else Convert.ChangeType(...)
                    var fallbackDefaultValue = Expression.Condition(
                        isNullCheck,
                        defaultValue,  // Use default value if null
                        conversion);     // Otherwise, use the converted value

                    var convertedValue = Expression.Convert(fallbackDefaultValue, targetProperty.PropertyType);

                    yield return Expression.Bind(targetProperty, convertedValue);
                }
            }
        }
    }
}