namespace KISS.FluentSqlBuilder.Composite;

/// <summary>
///     A context for storing reusable instances used in expression tree construction.
/// </summary>
public sealed partial record CompositeQuery
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

                yield return Expression.Bind(targetProperty, TypeUtils.ChangeType(sourceValue, targetProperty.PropertyType));
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
}
