namespace KISS.QueryBuilder.Component;

public sealed record DirectionalSortDefinition<TEntity>(FieldDefinition<TEntity> Field, SortDirection Direction)
{

}