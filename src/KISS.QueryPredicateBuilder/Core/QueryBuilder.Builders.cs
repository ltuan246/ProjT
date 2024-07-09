namespace KISS.QueryPredicateBuilder.Core;

public sealed partial class QueryBuilder
{
    private Type Entity { get; set; } = default!;

    private IEnumerable<PropertyInfo> Properties { get; set; } = [];

    public void SelectFrom<TEntity>()
    {
        Entity = typeof(TEntity);
        Properties = Entity.GetProperties();
    }
}