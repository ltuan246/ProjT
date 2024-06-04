namespace KISS.QueryBuilder.DataAccess;

public sealed class GenericRepository<TEntity>(IDbTransaction transaction)
{
    private IDbConnection Connection { get; init; } = transaction.Connection ?? default!;

    private IComponent? Filter { get; set; }

    public void Find(IComponent filter)
    {
        Filter = filter;
    }

    public IEnumerable<TEntity> GetList()
    {
        var query = GetQuery();
        var result = Connection.Query<TEntity>(query);
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