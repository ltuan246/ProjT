namespace KISS.QueryBuilder.Tests.UnitTests;

public sealed class ExpressionTranslatorTests
{
    private Translator Trans { get; } = new();

    [Fact]
    public void Translate_BinaryExpression_TranslatesToSql()
    {
        var param = Expression.Parameter(typeof(HourlyWeather), "x");
        var binaryExpr = Expression.GreaterThan(
            Expression.Property(param, "TempC"),
            Expression.Constant((double)18)
        );

        Trans.Visit(binaryExpr);

        Assert.Equal("TempC > 18", Trans.TranslatedSql);
    }

    [Fact]
    public void Translate_UnaryExpression_TranslatesToSql()
    {
        var param = Expression.Parameter(typeof(HourlyWeather), "x");
        var unaryExpr = Expression.Not(Expression.Property(param, "IsDay"));

        Trans.Visit(unaryExpr);

        Assert.Equal("NOT IsDay", Trans.TranslatedSql);
    }

    [Fact]
    public void Translate_MemberExpression_TranslatesToSql()
    {
        var param = Expression.Parameter(typeof(Location), "x");
        var memberExpr = Expression.Property(param, "Id");

        Trans.Visit(memberExpr);

        Assert.Equal("Id", Trans.TranslatedSql);
    }

    [Fact]
    public void Translate_ConstantExpression_TranslatesToSql()
    {
        var constantExpr = Expression.Constant(42);

        Trans.Visit(constantExpr);

        Assert.Equal("42", Trans.TranslatedSql);
    }

    [Fact]
    public void Translate_NewExpression_TranslatesToSql()
    {
        var newExpr = Expression.New(typeof(Location));

        Trans.Visit(newExpr);

        Assert.Equal("NEW Location", Trans.TranslatedSql);
    }

    [Fact]
    public void Translate_MemberInitExpression_TranslatesToSql()
    {
        var memberInitExpr = Expression.MemberInit(
            Expression.New(typeof(Location)),
            Expression.Bind(typeof(Location).GetProperty("Id")!,
                Expression.Constant("23202fb3-a995-4e7e-a91e-eb192e2e9872"))
        );

        Trans.Visit(memberInitExpr);

        Assert.Equal("NEW Location { Id = '23202fb3-a995-4e7e-a91e-eb192e2e9872' }", Trans.TranslatedSql);
    }

    [Fact]
    public void Translate_MethodCallExpression_TranslatesToSql()
    {
        var param = Expression.Parameter(typeof(Location), "x");
        var methodCallExpr = Expression.Call(
            Expression.Property(param, "Id"),
            typeof(string).GetMethod("ToLower", Type.EmptyTypes)!
        );

        Trans.Visit(methodCallExpr);

        Assert.Equal("ToLower(Id)", Trans.TranslatedSql);
    }

    [Fact]
    public void Translate_LambdaExpression_TranslatesToSql()
    {
        var param = Expression.Parameter(typeof(Location), "x");
        var binaryExpr = Expression.GreaterThan(
            Expression.Property(param, "Latitude"),
            Expression.Constant((double)18)
        );
        var lambdaExpr = Expression.Lambda(binaryExpr, param);

        Trans.Visit(lambdaExpr);

        Assert.Equal("(x) => Latitude > 18", Trans.TranslatedSql);
    }
}
