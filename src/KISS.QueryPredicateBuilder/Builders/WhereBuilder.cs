namespace KISS.QueryPredicateBuilder.Builders;

public sealed class WhereBuilder<TEntity>
{
    public OperatorFilterDefinition Eq<TField>(Expression<Func<TEntity, TField>> field, TField value)
        => new();
}