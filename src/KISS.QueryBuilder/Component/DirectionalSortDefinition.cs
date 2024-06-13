namespace KISS.QueryBuilder.Component;

public sealed record DirectionalSortDefinition<TComponent>(FieldDefinition<TComponent> Field, SortDirection Direction)
{

}