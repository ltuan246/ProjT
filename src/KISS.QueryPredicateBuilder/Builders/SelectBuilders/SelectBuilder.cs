namespace KISS.QueryPredicateBuilder.Builders.SelectBuilders;

public sealed record SelectBuilder<TEntity>
{
    private static Type Entity => typeof(TEntity);
    private static IEnumerable<PropertyInfo> Properties => Entity.GetProperties();

    private List<string> Columns { get; } = [];
    private List<string> ExColumns { get; } = [];

    public SelectBuilder<TEntity> Include<TField>(Expression<Func<TEntity, TField>> field)
    {
        Columns.Add(new ExpressionFieldDefinition<TEntity, TField>(field));
        return this;
    }

    public SelectBuilder<TEntity> Exclude<TField>(Expression<Func<TEntity, TField>> field)
    {
        ExColumns.Add(new ExpressionFieldDefinition<TEntity, TField>(field));
        return this;
    }

    public ProjectionDefinition Build()
        => new($"SELECT {string.Join(", ", GetFields()):raw} FROM {Entity.Name:raw}s");

    private IEnumerable<string> GetFields()
    {
        List<string> cols = Columns.Count switch
        {
            0 => Properties.Select(p => p.Name).ToList(),
            _ => Columns
        };

        foreach (var col in cols)
        {
            if (ExColumns.Contains(col))
            {
                continue;
            }

            yield return $"[{col}]";
        }

        Columns.Clear();
        ExColumns.Clear();
    }
}