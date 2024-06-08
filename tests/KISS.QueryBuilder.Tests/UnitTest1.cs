namespace KISS.QueryBuilder.Tests;

public class UnitTest1 : IDisposable
{
    private SqliteConnection Connection { get; init; }
    private ApplicationDbContext Context { get; init; }
    private GenericRepository<User> Repo { get; init; }

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
        string fileName = "Assets/GlobalWeatherRepository.csv";
        var weather = CsvAssists.FromCsv<Weather>(fileName);

        const string query = "SELECT * FROM Users";
        DbConnection conn = Context.Database.GetDbConnection();
        IEnumerable<User> users = conn.Query<User>(query);
        Assert.True(users.Any());
    }

    [Fact]
    public void Test2()
    {
        var filter = Repo.Filter.Eq(t => t.Name, "Tuna");
        IEnumerable<User> users = Repo.Query(filter);
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