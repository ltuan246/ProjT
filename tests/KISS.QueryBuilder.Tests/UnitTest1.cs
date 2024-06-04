using Dapper;

namespace KISS.QueryBuilder.Tests;

public class UnitTest1 : ContextFixture
{
    [Fact]
    public void Test1()
    {
        using SqliteConnection connection = CreateConnection();
        using var context = CreateContext(connection);

        Assert.True(context.Users.Any());

        var query = $"SELECT * FROM Users";
        var users = connection.Query<User>(query);

        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.And(builder.Eq(t => t.AsString, "a"), builder.Eq(t => t.AsString, "b"));
        var result = filter.Render();

        // repo!.Find(filter);
        // var rs = repo.GetList();
    }
}