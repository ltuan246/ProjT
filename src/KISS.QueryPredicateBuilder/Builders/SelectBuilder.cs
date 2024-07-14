namespace KISS.QueryPredicateBuilder.Builders;

public sealed record SelectBuilder<TEntity>
{
    public SelectBuilder<TEntity> Include<TField>(Expression<Func<TEntity, TField>> field)
    {
        return this;
    }

    // public SelectBuilder<TEntity> Exclude<TField>(Expression<Func<TEntity, TField>> field)
    //     => new((new ExpressionFieldDefinition<TEntity, TField>(field), false));
}