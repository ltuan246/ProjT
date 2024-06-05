namespace KISS.QueryBuilder.DataAccess;

public sealed record GenericRepository<TEntity>(DbContext Context)
{
    public FilterDefinitionBuilder<TEntity> Filter { get; } = Builders<TEntity>.Filter;

    private IDbConnection GetConnection()
    {
        var connection = Context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        return connection;
    }

    public IEnumerable<TEntity> Query(IComponent filter)
    {
        string render = CompositeQueries.Render(filter);

        StringBuilder builder = new();
        builder.Append($"SELECT * FROM {typeof(TEntity).Name}s");
        builder.AppendLine(render);
        string query = builder.ToString();

        var connection = GetConnection();
        return connection.Query<TEntity>(query);
    }

    public IEnumerable<TEntity> GetList()
    {
        string query = $"SELECT * FROM {typeof(TEntity).Name}s";
        var connection = GetConnection();
        return connection.Query<TEntity>(query);
    }
}