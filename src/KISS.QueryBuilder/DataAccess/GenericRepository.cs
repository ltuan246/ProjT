namespace KISS.QueryBuilder.DataAccess;

public sealed class GenericRepository<TEntity>(DbContext dbContext)
{
    private IDbConnection Connection { get; init; } = dbContext.Database.GetDbConnection();

    private IComponent? Filter { get; set; }

    public void Find(IComponent filter)
    {
        Filter = filter;
    }

    public IEnumerable<TEntity> GetList()
    {
        // var query = GetQuery();
        if (Connection.State != ConnectionState.Open)
        {
            Connection.Open();
        }

        var result = Connection.Query<TEntity>($"SELECT * FROM Users");
        return result;
    }

    public async Task<IEnumerable<TEntity>> GetListAsync()
    {
        var query = GetQuery();
        var result = await Connection.QueryAsync<TEntity>(query);
        return result;
    }

    private string GetQuery()
    {
        var query = CompositeQueries.Render(Filter!);
        return query;
    }
}