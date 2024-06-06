namespace KISS.QueryBuilder.DataAccess;

public sealed record GenericRepository<TEntity>(DbContext Context)
{
    public FilterDefinitionBuilder<TEntity> Filter => Builders<TEntity>.Filter;
    private static Type Entity => typeof(TEntity);
    private static IEnumerable<PropertyInfo> Properties => Entity.GetProperties();

    private DbConnection GetConnection()
    {
        DbConnection connection = Context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        return connection;
    }

    public IEnumerable<TEntity> Query(IComponent filter)
    {
        string render = CompositeQueries.Render(filter);
        string[] propsName = Properties.Select(p => p.Name).ToArray();

        StringBuilder builder = new();
        builder.Append($"SELECT {string.Join(", ", propsName)} FROM {typeof(TEntity).Name}s");
        builder.Append($" WHERE {render}");
        string query = builder.ToString();

        DbConnection connection = GetConnection();
        return connection.Query<TEntity>(query);
    }

    public IEnumerable<TEntity> GetList()
    {
        string query = $"SELECT * FROM {Entity.Name}s";
        DbConnection connection = GetConnection();
        return connection.Query<TEntity>(query);
    }
}