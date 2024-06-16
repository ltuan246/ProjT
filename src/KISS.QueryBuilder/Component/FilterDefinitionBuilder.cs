namespace KISS.QueryBuilder.Component;

public sealed record FilterDefinitionBuilder<TEntity>
{
    /// <summary>
    /// Creates an equality filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>An equality filter.</returns>
    public OperatorFilterDefinition<TEntity, TField> Eq<TField>(
        Expression<Func<TEntity, TField>> field,
        [DisallowNull] TField value)
        => new(ComparisonOperator.Equals, new ExpressionFieldDefinition<TEntity, TField>(field), value);

    /// <summary>
    /// Creates a not equal filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A not equal filter.</returns>
    public OperatorFilterDefinition<TEntity, TField> Ne<TField>(
        Expression<Func<TEntity, TField>> field,
        [DisallowNull] TField value)
        => new(ComparisonOperator.NotEquals, new ExpressionFieldDefinition<TEntity, TField>(field), value);

    /// <summary>
    /// Creates a greater than filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A greater than filter.</returns>
    public OperatorFilterDefinition<TEntity, TField> Gt<TField>(
        Expression<Func<TEntity, TField>> field,
        [DisallowNull] TField value)
        => new(ComparisonOperator.Greater, new ExpressionFieldDefinition<TEntity, TField>(field), value);

    /// <summary>
    /// Creates a greater than or equal filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A greater than or equal filter.</returns>
    public OperatorFilterDefinition<TEntity, TField> Gte<TField>(
        Expression<Func<TEntity, TField>> field,
        [DisallowNull] TField value)
        => new(ComparisonOperator.GreaterOrEquals, new ExpressionFieldDefinition<TEntity, TField>(field), value);

    /// <summary>
    /// Creates a less than filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A less than filter.</returns>
    public OperatorFilterDefinition<TEntity, TField> Lt<TField>(
        Expression<Func<TEntity, TField>> field,
        [DisallowNull] TField value)
        => new(ComparisonOperator.Less, new ExpressionFieldDefinition<TEntity, TField>(field), value);

    /// <summary>
    /// Creates a less than or equal filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A less than or equal filter.</returns>
    public OperatorFilterDefinition<TEntity, TField> Lte<TField>(
        Expression<Func<TEntity, TField>> field,
        [DisallowNull] TField value)
        => new(ComparisonOperator.LessOrEquals, new ExpressionFieldDefinition<TEntity, TField>(field), value);

    /// <summary>
    /// Creates an in filter for an array field.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>An in filter.</returns>
    public SingleItemAsArrayOperatorFilterDefinition<TEntity, TField> AnyIn<TField>(
        Expression<Func<TEntity, TField>> field,
        params TField[] value)
        => new(SingleItemAsArrayOperator.Contains, new ExpressionFieldDefinition<TEntity, TField>(field), value);

    /// <summary>
    /// Creates a not in filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A not in filter.</returns>
    public SingleItemAsArrayOperatorFilterDefinition<TEntity, TField> NotIn<TField>(
        Expression<Func<TEntity, TField>> field,
        params TField[] value)
        => new(SingleItemAsArrayOperator.NotContains, new(field), value);

    /// <summary>
    /// Creates an and filter.
    /// </summary>
    /// <param name="filterDefinitions">The filters.</param>
    /// <returns>A filter.</returns>
    public AndFilterDefinition And(params IQuerying[] filterDefinitions)
        => new(filterDefinitions);

    /// <summary>
    /// Creates an and filter.
    /// </summary>
    /// <param name="filterDefinitions">The filters.</param>
    /// <returns>An or filter.</returns>
    public OrFilterDefinition Or(params IQuerying[] filterDefinitions)
        => new(filterDefinitions);
}