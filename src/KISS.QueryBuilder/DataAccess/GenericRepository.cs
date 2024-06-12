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
        const string sqlWhereClause = """
                                          SELECT {0} FROM {1}s
                                          WHERE {2}
                                      """;

        (string conditions, Dictionary<string, object> queryParameters) = CompositeQueries.Render(filter);
        string[] propsName = Properties.Select(p => p.Name).ToArray();
        string columns = string.Join(", ", propsName);
        string table = Entity.Name;

        StringBuilder builder = new();
        builder.AppendFormat(sqlWhereClause, columns, table, conditions);
        string query = builder.ToString();

        DbConnection connection = GetConnection();
        return connection.Query<TEntity>(query, queryParameters);
    }

    public IEnumerable<TEntity> GetList()
    {
        string query = $"SELECT * FROM {Entity.Name}s";
        DbConnection connection = GetConnection();
        return connection.Query<TEntity>(query);
    }
}