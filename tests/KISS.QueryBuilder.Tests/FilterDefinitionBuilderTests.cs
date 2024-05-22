using System;

namespace KISS.QueryBuilder.Tests;

public class FilterDefinitionBuilderTests
{
    [Fact]
    public void Eq()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Eq(t => t.Name, "a");
        var result = filter.Render();
        Assert.Equal("Name = a", result);
    }

    [Fact]
    public void Ne()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Ne(t => t.Name, "a");
        var result = filter.Render();
        Assert.Equal("Name <> a", result);
    }

    [Fact]
    public void Gt()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Gt(t => t.Name, "a");
        var result = filter.Render();
        Assert.Equal("Name > a", result);
    }

    [Fact]
    public void Gte()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Gte(t => t.Name, "a");
        var result = filter.Render();
        Assert.Equal("Name >= a", result);
    }

    [Fact]
    public void Lt()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Lt(t => t.Name, "a");
        var result = filter.Render();
        Assert.Equal("Name < a", result);
    }

    [Fact]
    public void Lte()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Lte(t => t.Name, "a");
        var result = filter.Render();
        Assert.Equal("Name <= a", result);
    }

    [Fact]
    public void AnyIn()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.AnyIn(t => t.Name, ["a", "b", "c"]);
        var result = filter.Render();
        Assert.Equal("Name IN (a,b,c)", result);
    }

    [Fact]
    public void Nin()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Nin(t => t.Name, ["a", "b", "c"]);
        var result = filter.Render();
        Assert.Equal("Name NOT IN (a,b,c)", result);
    }

    [Fact]
    public void And()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.And(builder.Eq(t => t.Name, "a"), builder.Eq(t => t.Name, "b"));
        var result = filter.Render();
        Assert.Equal("Name = a AND Name = b", result);
    }

    [Fact]
    public void Or()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Or(builder.Eq(t => t.Name, "a"), builder.Eq(t => t.Name, "b"));
        var result = filter.Render();
        Assert.Equal("Name = a OR Name = b", result);
    }

    [Fact]
    public void Build_InvalidInput_ShouldThrowNotSupportedException()
    {
        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.Eq(t => "", "a");
        Assert.Throws<NotSupportedException>(() => filter.Render());
    }
}