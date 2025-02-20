namespace KISS.FluentSqlBuilder.Visitors.QueryComponent.Components;

/// <summary>
///     A builder for a <c>FROM</c> clause.
/// </summary>
/// <param name="Composite">The structure of the fluent SQL builder.</param>
public sealed record SelectFromTranslator(ICompositeQuery Composite) : ExpressionTranslator
{
    /// <inheritdoc />
    protected override void Translate(ConstantExpression constantExpression)
    {
        Composite.Append(
            $"{((Type)constantExpression.Value!).Name}s {Composite.GetAliasMapping((Type)constantExpression.Value)}");
        Composite.RetrievePropertyAssignmentProcessing.AddRange([.. CreateBindings((Type)constantExpression.Value)]);
    }

    /// <summary>
    ///     Creates a collection of <see cref="MemberBinding" /> instances
    ///     that map properties from the source type to the target type.
    /// </summary>
    /// <returns>
    ///     An enumerable collection of <see cref="MemberBinding" /> objects,
    ///     where each binding represents the assignment of a source property value to a target property.
    /// </returns>
    private IEnumerable<MemberAssignment> CreateBindings(Type sourceEntity)
    {
        var sourceProperties = sourceEntity.GetProperties().Where(p => p.CanWrite).ToList();
        foreach (var sourceProperty in sourceProperties)
        {
            var targetProperty = Composite.RetrieveEntity.GetProperty(sourceProperty.Name);
            if (targetProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
            {
                var sourceValue = Expression.Property(Composite.DapperRowVariable, "Item", Expression.Constant($"Extend0_{sourceProperty.Name}"));

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
