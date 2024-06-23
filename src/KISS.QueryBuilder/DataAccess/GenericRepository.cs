namespace KISS.QueryBuilder.DataAccess;

public sealed record GenericRepository<TEntity>(DbContext Context)
{
    public FilterDefinitionBuilder<TEntity> Filter { get; } = new();

    public SortDefinitionBuilder<TEntity> Sort { get; } = new();

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

    public List<TEntity> Query(params IQuerying[] queries)
    {
        const string sqlWhereClause = "SELECT {0} FROM {1}s {2}";

        Builder<TEntity> queryBuilder = new(queries);
        (string conditions, Dictionary<string, object> queryParameters) = queryBuilder.Render();
        string[] propsName = Properties.Select(p => p.Name).ToArray();
        string columns = string.Join(", ", propsName);
        string table = Entity.Name;

        StringBuilder builder = new();
        builder.AppendFormat(sqlWhereClause, columns, table, conditions);
        string query = builder.ToString();

        DbConnection connection = GetConnection();
        return connection.Query<TEntity>(query, queryParameters).ToList();
    }

    public List<TEntity> GetList()
    {
        const string sqlSelectClause = "SELECT {0} FROM {1}s";
        string[] propsName = Properties.Select(p => p.Name).ToArray();
        string columns = string.Join(", ", propsName);
        string table = Entity.Name;

        StringBuilder builder = new();
        builder.AppendFormat(sqlSelectClause, columns, table);
        string query = builder.ToString();

        DbConnection connection = GetConnection();
        return connection.Query<TEntity>(query).ToList();
    }

    public int Count()
    {
        const string sqlCount = "SELECT COUNT(1) FROM {0}s";
        string table = Entity.Name;

        StringBuilder builder = new();
        builder.AppendFormat(sqlCount, table);
        string query = builder.ToString();

        DbConnection connection = GetConnection();
        return connection.ExecuteScalar<int>(query);
    }
}