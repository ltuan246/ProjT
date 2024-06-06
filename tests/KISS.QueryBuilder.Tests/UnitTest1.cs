using System.Collections.Generic;
using System.Data.Common;
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
        const string query = "SELECT * FROM Users";
        DbConnection conn = Context.Database.GetDbConnection();
        IEnumerable<User> users = conn.Query<User>(query);
        Assert.True(users.Any());
    }

    [Fact]
    public void Test2()
    {
        var filter = Repo.Filter.Eq(t => t.Id, "a");
        IEnumerable<User> users = Repo.Query(filter);
        Assert.True(users.Any());
    }
}