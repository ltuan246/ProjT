namespace KISS.QueryPredicateBuilder.Builders.WhereBuilders;

/// <summary>
///     Defines the where builder type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public sealed record WhereBuilder<TEntity>
{
    private static FormattableString BuildClause<TField>(
        Expression<Func<TEntity, TField>> field,
        string comparisonOperator,
        TField value)
        => $"[{(string)new ExpressionFieldDefinition<TEntity, TField>(field):raw}] {comparisonOperator:raw} {value}";

    /// <summary>
    ///     Creates an equality filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>An equality filter.</returns>
    public OperatorFilterDefinition Eq<TField>(Expression<Func<TEntity, TField>> field, TField value)
        => new(BuildClause(field, ComparisonOperator.AreEqual, value));

    /// <summary>
    ///     Creates a not equal filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A not equal filter.</returns>
    public OperatorFilterDefinition Ne<TField>(
        Expression<Func<TEntity, TField>> field,
        [DisallowNull] TField value)
        => new(BuildClause(field, ComparisonOperator.NotEqual, value));

    /// <summary>
    ///     Creates a greater than filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A greater than filter.</returns>
    public OperatorFilterDefinition Gt<TField>(
        Expression<Func<TEntity, TField>> field,
        [DisallowNull] TField value)
        => new(BuildClause(field, ComparisonOperator.GreaterThan, value));

    /// <summary>
    ///     Creates a greater than or equal filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A greater than or equal filter.</returns>
    public OperatorFilterDefinition Gte<TField>(
        Expression<Func<TEntity, TField>> field,
        [DisallowNull] TField value)
        => new(BuildClause(field, ComparisonOperator.GreaterThanOrEqualTo, value));

    /// <summary>
    ///     Creates a less than filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A less than filter.</returns>
    public OperatorFilterDefinition Lt<TField>(
        Expression<Func<TEntity, TField>> field,
        [DisallowNull] TField value)
        => new(BuildClause(field, ComparisonOperator.LessThan, value));

    /// <summary>
    ///     Creates a less than or equal filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A less than or equal filter.</returns>
    public OperatorFilterDefinition Lte<TField>(
        Expression<Func<TEntity, TField>> field,
        [DisallowNull] TField value)
        => new(BuildClause(field, ComparisonOperator.LessThanOrEqualTo, value));

    /// <summary>
    ///     Creates an in filter for an array field.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="values">The values.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>An in filter.</returns>
    public SingleItemAsArrayOperatorFilterDefinition AnyIn<TField>(
        Expression<Func<TEntity, TField>> field,
        params TField[] values)
        => new($"[{(string)new ExpressionFieldDefinition<TEntity, TField>(field):raw}] IN {values}");

    /// <summary>
    ///     Creates a not in filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="values">The values.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>A not in filter.</returns>
    public SingleItemAsArrayOperatorFilterDefinition NotIn<TField>(
        Expression<Func<TEntity, TField>> field,
        params TField[] values)
        => new($"[{(string)new ExpressionFieldDefinition<TEntity, TField>(field):raw}] NOT IN {values}");

    /// <summary>
    ///     Creates the between filter.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="beginValue">The begin values.</param>
    /// <param name="endValue">The end values.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>The between filter.</returns>
    public RangeFilterDefinition Within<TField>(
        Expression<Func<TEntity, TField>> field,
        [DisallowNull] TField beginValue,
        [DisallowNull] TField endValue)
        => new(
            $"[{(string)new ExpressionFieldDefinition<TEntity, TField>(field):raw}] BETWEEN {beginValue} AND {endValue}");

    /// <summary>
    ///     Creates an and filter.
    /// </summary>
    /// <param name="filterDefinitions">The filters.</param>
    /// <returns>A filter.</returns>
    public CombinedFilterDefinition And(params IComponent[] filterDefinitions)
        => new(ClauseAction.Where, ClauseConstants.Where.AndSeparator, filterDefinitions);

    /// <summary>
    ///     Creates an or filter.
    /// </summary>
    /// <param name="filterDefinitions">The filters.</param>
    /// <returns>An or filter.</returns>
    public CombinedFilterDefinition Or(params IComponent[] filterDefinitions)
        => new(ClauseAction.Where, ClauseConstants.Where.OrSeparator, filterDefinitions);
}

/// <summary>
///     Used in conditions that compares one expression with another.
/// </summary>
public sealed class ComparisonOperator
{
    /// <summary>
    ///     Equal.
    /// </summary>
    public const string AreEqual = "=";

    /// <summary>
    ///     Not Equal.
    /// </summary>
    public const string NotEqual = "<>";

    /// <summary>
    ///     Greater Than.
    /// </summary>
    public const string GreaterThan = ">";

    /// <summary>
    ///     Greater Than Or Equal To.
    /// </summary>
    public const string GreaterThanOrEqualTo = ">=";

    /// <summary>
    ///     Less Than.
    /// </summary>
    public const string LessThan = "<";

    /// <summary>
    ///     Less Than Or Equal To.
    /// </summary>
    public const string LessThanOrEqualTo = "<=";
}
