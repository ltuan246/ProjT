using System.Linq;

namespace KISS.QueryBuilder.Tests;

public class UnitTest1 : IDisposable
{
    private SqliteConnection Connection { get; init; }
    private ApplicationDbContext Context { get; init; }
    private GenericRepository<User> Repo { get; init; }

    public UnitTest1()
    {
        Connection = new SqliteConnection("datasource=:memory:");
        Connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
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
        var query = $"SELECT * FROM Users";
        var conn = Context.Database.GetDbConnection();
        var users = conn.Query<User>(query);
        Assert.True(users.Any());
    }

    [Fact]
    public void Test2()
    {
        var filter = Repo.Filter.Eq(t => t.Id, "a");
        var users = Repo.Query(filter);
        Assert.True(users.Any());
    }
}