namespace KISS.QueryBuilder.Component;

public sealed record FilterDefinitionBuilder<TComponent>
{
    /// <summary>
    /// Creates an equality filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>An equality filter.</returns>
    public OperatorFilterDefinition<TComponent, TField> Eq<TField>(
        Expression<Func<TComponent, TField>> field,
        TField value)
        => new(ComparisonOperator.Equals, new(field), value);

    /// <summary>
    /// Creates a not equal filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A not equal filter.</returns>
    public OperatorFilterDefinition<TComponent, TField> Ne<TField>(
        Expression<Func<TComponent, TField>> field,
        TField value)
        => new(ComparisonOperator.NotEquals, new(field), value);

    /// <summary>
    /// Creates a greater than filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A greater than filter.</returns>
    public OperatorFilterDefinition<TComponent, TField> Gt<TField>(
        Expression<Func<TComponent, TField>> field,
        TField value)
        => new(ComparisonOperator.Greater, new(field), value);

    /// <summary>
    /// Creates a greater than or equal filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A greater than or equal filter.</returns>
    public OperatorFilterDefinition<TComponent, TField> Gte<TField>(
        Expression<Func<TComponent, TField>> field,
        TField value)
        => new(ComparisonOperator.GreaterOrEquals, new(field), value);

    /// <summary>
    /// Creates a less than filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A less than filter.</returns>
    public OperatorFilterDefinition<TComponent, TField> Lt<TField>(
        Expression<Func<TComponent, TField>> field,
        TField value)
        => new(ComparisonOperator.Less, new(field), value);

    /// <summary>
    /// Creates a less than or equal filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A less than or equal filter.</returns>
    public OperatorFilterDefinition<TComponent, TField> Lte<TField>(
        Expression<Func<TComponent, TField>> field,
        TField value)
        => new(ComparisonOperator.LessOrEquals, new(field), value);

    /// <summary>
    /// Creates an in filter for an array field.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>An in filter.</returns>
    public SingleItemAsArrayOperatorFilterDefinition<TComponent, TField> AnyIn<TField>(
        Expression<Func<TComponent, TField>> field,
        params TField[] value)
        => new(SingleItemAsArrayOperator.Contains, new(field), value);

    /// <summary>
    /// Creates a not in filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A not in filter.</returns>
    public SingleItemAsArrayOperatorFilterDefinition<TComponent, TField> Nin<TField>(
        Expression<Func<TComponent, TField>> field,
        params TField[] value)
        => new(SingleItemAsArrayOperator.NotContains, new(field), value);

    /// <summary>
    /// Creates an and filter.
    /// </summary>
    /// <param name="filterDefinitions">The filters.</param>
    /// <returns>A filter.</returns>
    public AndFilterDefinition And(params IComponent[] filterDefinitions)
        => new(filterDefinitions);

    /// <summary>
    /// Creates an and filter.
    /// </summary>
    /// <param name="filterDefinitions">The filters.</param>
    /// <returns>An or filter.</returns>
    public OrFilterDefinition Or(params IComponent[] filterDefinitions)
        => new(filterDefinitions);
}