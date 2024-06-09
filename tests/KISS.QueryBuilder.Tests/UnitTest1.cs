namespace KISS.QueryBuilder.Tests;

public class UnitTest1 : IDisposable
{
    private SqliteConnection Connection { get; init; }
    private ApplicationDbContext Context { get; init; }
    private GenericRepository<Weather> Repo { get; init; }

    public UnitTest1()
    {
        SqlMapper.AddTypeHandler(new GuidHandler());

        // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/dapper-limitations
        const string connectionString = "datasource=:memory:";
        Connection = new SqliteConnection(connectionString);
        Connection.Open();

        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(Connection)
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();

        Repo = new(Context);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Connection.Close();
        Connection.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Test1()
    {
        const string query = "SELECT * FROM Weathers WHERE Id = @Id";
        Dictionary<string, object> dic = new() { ["@Id"] = new Guid("2DFA8730-2541-11EF-83FE-B1C709C359B7") };
        DbConnection conn = Context.Database.GetDbConnection();
        IEnumerable<Weather> users = conn.Query<Weather>(query, new DynamicParameters(dic));
        Assert.True(users.Any());
    }

    [Fact]
    public void Test2()
    {
        var filter = Repo.Filter.Eq(t => t.Id, new("2DFA8730-2541-11EF-83FE-B1C709C359B7"));
        IEnumerable<Weather> users = Repo.Query(filter);
        Assert.True(users.Any());
    }
}


public abstract class SqliteTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    // Parameters are converted by Microsoft.Data.Sqlite
    public override void SetValue(IDbDataParameter parameter, T? value)
        => parameter.Value = value;
}

public class GuidHandler : SqliteTypeHandler<Guid>
{
    public override Guid Parse(object value)
        => Guid.Parse((string)value);
}