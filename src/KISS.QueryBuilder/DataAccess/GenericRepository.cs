namespace KISS.QueryBuilder.DataAccess;

public sealed class GenericRepository<T>(DapperContext context)
{
    private DapperContext Context { get; init; } = context;

    private IComponent? Filter { get; set; }

    public void Find(IComponent filter)
    {
        Filter = filter;
    }

    public IEnumerable<T> GetList()
    {
        var query = GetQuery();
        using var connection = Context.CreateConnection();
        var result = connection.Query<T>(query);
        return result;
    }

    public async Task<IEnumerable<T>> GetListAsync()
    {
        var query = GetQuery();
        using var connection = Context.CreateConnection();
        var result = await connection.QueryAsync<T>(query);
        return result;
    }

    private string GetQuery()
    {
        var query = CompositeQueries.Render(Filter!);
        return query;
    }
}