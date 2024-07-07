namespace KISS.QueryPredicateBuilder.Builders;

public sealed record WhereBuilder<TEntity>
{
    public OperatorFilterDefinition Eq<TField>(Expression<Func<TEntity, TField>> field, TField value)
        => new($"[{(string)new ExpressionFieldDefinition<TEntity, TField>(field):raw}] = {value}");

    /// <summary>
    /// Creates an and filter.
    /// </summary>
    /// <param name="filterDefinitions">The filters.</param>
    /// <returns>A filter.</returns>
    public CombinedFilterDefinition And(params IComponent[] filterDefinitions)
        => new(ClauseAction.Where, ClauseConstants.Where.AndSeparator, filterDefinitions);

    /// <summary>
    /// Creates an or filter.
    /// </summary>
    /// <param name="filterDefinitions">The filters.</param>
    /// <returns>An or filter.</returns>
    public CombinedFilterDefinition Or(params IComponent[] filterDefinitions)
        => new(ClauseAction.Where, ClauseConstants.Where.OrSeparator, filterDefinitions);
}