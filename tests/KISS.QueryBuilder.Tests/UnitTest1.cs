namespace KISS.QueryBuilder.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var builder = Builders<Test>.Filter;
        var filter = builder.Eq(t => t.Name, "a");
        var result = filter.Render();
    }
}

public class Test
{
    public string Name { get; }
}