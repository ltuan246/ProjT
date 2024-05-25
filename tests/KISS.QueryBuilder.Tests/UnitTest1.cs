namespace KISS.QueryBuilder.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.And(builder.Eq(t => t.AsString, "a"), builder.Eq(t => t.AsString, "b"));
        var result = filter.Render();
    }
}