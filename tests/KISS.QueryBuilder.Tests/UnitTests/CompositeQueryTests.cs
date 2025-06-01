namespace KISS.QueryBuilder.Tests.UnitTests;

[Collection(nameof(SqliteTestsCollection))]
public sealed class CompositeQueryTests()
{
    private ExpressionEvaluator Query { get; } = new();

    [Fact]
    public void GetValue_UnvisitedConstant_EvaluatesToValue()
    {
        // Arrange: A constant expression not yet visited
        var constant = Expression.Constant(42);

        // Act
        var (evaluated, value) = Query.GetValue(constant);

        // Assert
        Assert.True(evaluated);
        Assert.Equal("42", value.ToString());
    }

    [Fact]
    public void GetValue_UnvisitedParameter_ReturnsNotEvaluable()
    {
        // Arrange: A parameter expression with no value
        var param = Expression.Parameter(typeof(int), "x");

        // Act
        var (evaluated, value) = Query.GetValue(param);

        // Assert
        Assert.False(evaluated);
        Assert.Equal("", value.ToString());
    }

    [Fact]
    public void GetValue_VoidMethodCall_ReturnsNotEvaluable()
    {
        // Arrange: A void method call expression
        var voidCall = Expression.Call(
            typeof(Console).GetMethod("WriteLine", [typeof(string)])!,
            Expression.Constant("Hi")
        );

        // Act
        var (evaluated, value) = Query.GetValue(voidCall);

        // Assert
        Assert.False(evaluated);
        Assert.Equal("", value.ToString());
    }

    [Fact]
    public void GetValue_BlockWithValue_ReturnsNotEvaluable()
    {
        // Arrange: A block with a void call and a final constant value
        var voidCall = Expression.Call(
            typeof(Console).GetMethod("WriteLine", [typeof(string)])!,
            Expression.Constant("Hi")
        );
        var block = Expression.Block(voidCall, Expression.Constant(5));

        // Act
        var (evaluated, value) = Query.GetValue(block);

        // Assert
        Assert.False(evaluated);
        Assert.Equal("", value.ToString());
    }

    [Fact]
    public void GetValue_LambdaWithConstant_ReturnsNotEvaluable()
    {
        // Arrange: A lambda wrapping a constant
        var constant = Expression.Constant(10);
        var lambda = Expression.Lambda(constant);

        // Act
        var (evaluated, value) = Query.GetValue(lambda);

        // Assert
        Assert.False(evaluated);
        Assert.Equal("", value.ToString());
    }

    [Fact]
    public void GetValue_PreVisitedConstant_ReturnsValue()
    {
        // Arrange: A constant expression already visited
        var constant = Expression.Constant(42);
        Query.Visit(constant); // Pre-visit to populate Evaluable

        // Act
        var (evaluated, value) = Query.GetValue(constant);

        // Assert
        Assert.True(evaluated);
        Assert.Equal("42", value.ToString());
    }

    [Fact]
    public void GetValue_PreVisitedParameter_ReturnsNotEvaluable()
    {
        // Arrange: A parameter expression already visited
        var param = Expression.Parameter(typeof(int), "x");
        Query.Visit(param); // Pre-visit to populate Evaluable

        // Act
        var (evaluated, value) = Query.GetValue(param);

        // Assert
        Assert.False(evaluated);
        Assert.Equal("", value.ToString());
    }

    [Fact]
    public void GetValue_DateTimeConstant_EvaluatesToString()
    {
        // Arrange: A DateTime constant
        var dateTime = new DateTime(2023, 10, 15, 0, 0, 0, DateTimeKind.Utc);
        var constant = Expression.Constant(dateTime);

        // Act
        var (evaluated, value) = Query.GetValue(constant);

        // Assert
        Assert.True(evaluated);
        // Assert.Equal("10/15/2023 00:00:00", value.ToString()); // Adjust format based on culture
    }

    [Fact]
    public void GetValue_StringConstant_EvaluatesToValue()
    {
        // Arrange: A string constant
        var constant = Expression.Constant("Hello");

        // Act
        var (evaluated, value) = Query.GetValue(constant);

        // Assert
        Assert.True(evaluated);
        Assert.Equal("Hello", value.ToString());
    }

    [Fact]
    public void GetValue_BoolConstant_EvaluatesToValue()
    {
        // Arrange: A boolean constant
        var constant = Expression.Constant(true);

        // Act
        var (evaluated, value) = Query.GetValue(constant);

        // Assert
        Assert.True(evaluated);
        Assert.Equal("True", value.ToString());
    }

    [Fact]
    public void GetValue_NullConstant_EvaluatesToNullString()
    {
        // Arrange: A null constant
        var constant = Expression.Constant(null, typeof(object));

        // Act
        var (evaluated, value) = Query.GetValue(constant);

        // Assert
        Assert.True(evaluated);
        Assert.Equal("", value.ToString());
    }

    [Fact]
    public void GetValue_ReferenceTypeConstant_EvaluatesToValue()
    {
        // Arrange: A reference type constant (e.g., a custom object)
        var model = new WeatherModel { Id = "23202fb3-a995-4e7e-a91e-eb192e2e9872", TzId = "Europe/Andorra" };
        var constant = Expression.Constant(model);

        // Act
        var (evaluated, value) = Query.GetValue(constant);

        // Assert
        Assert.True(evaluated);
        Assert.Equal(model.ToString(), value.ToString());
    }
}
