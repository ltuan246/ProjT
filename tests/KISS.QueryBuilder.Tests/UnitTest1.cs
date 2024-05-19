namespace KISS.QueryBuilder.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var builder = Builders<Test>.Filter;
        var filter = builder.And(builder.Eq(t => t.Name, "a"), builder.Eq(t => t.Name, "b"));
        var result = filter.Render();
    }
}

public class Test
{
    public string Name { get; } = "";
}