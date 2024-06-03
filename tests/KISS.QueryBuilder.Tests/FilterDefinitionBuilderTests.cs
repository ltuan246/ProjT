namespace KISS.QueryBuilder.Tests;

public class FilterDefinitionBuilderTests
{
    [Fact]
    public void Eq()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Eq(t => t.AsString, "a");
        var result = filter.Render();
        Assert.Equal("AsString = a", result);
    }

    [Fact]
    public void Ne()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Ne(t => t.AsString, "a");
        var result = filter.Render();
        Assert.Equal("AsString <> a", result);
    }

    [Fact]
    public void Gt()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Gt(t => t.AsString, "a");
        var result = filter.Render();
        Assert.Equal("AsString > a", result);
    }

    [Fact]
    public void Gte()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Gte(t => t.AsString, "a");
        var result = filter.Render();
        Assert.Equal("AsString >= a", result);
    }

    [Fact]
    public void Lt()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Lt(t => t.AsString, "a");
        var result = filter.Render();
        Assert.Equal("AsString < a", result);
    }

    [Fact]
    public void Lte()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Lte(t => t.AsString, "a");
        var result = filter.Render();
        Assert.Equal("AsString <= a", result);
    }

    [Fact]
    public void AnyIn()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.AnyIn(t => t.AsString, ["a", "b", "c"]);
        var result = filter.Render();
        Assert.Equal("AsString IN (a,b,c)", result);
    }

    [Fact]
    public void Nin()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Nin(t => t.AsString, ["a", "b", "c"]);
        var result = filter.Render();
        Assert.Equal("AsString NOT IN (a,b,c)", result);
    }

    [Fact]
    public void And()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.And(builder.Eq(t => t.AsString, "a"), builder.Eq(t => t.AsString, "b"));
        var result = filter.Render();
        Assert.Equal("AsString = a AND AsString = b", result);
    }

    [Fact]
    public void Or()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Or(builder.Eq(t => t.AsString, "a"), builder.Eq(t => t.AsString, "b"));
        var result = filter.Render();
        Assert.Equal("AsString = a OR AsString = b", result);
    }

    [Fact]
    public void Build_InvalidInput_ShouldThrowNotSupportedException()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Eq(t => "", "a");
        Assert.Throws<NotSupportedException>(() => filter.Render());
    }
}