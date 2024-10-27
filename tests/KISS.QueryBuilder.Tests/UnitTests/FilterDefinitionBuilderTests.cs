namespace KISS.QueryBuilder.Tests.UnitTests;

[Collection(nameof(SqliteTestsCollection))]
public sealed class FilterDefinitionBuilderTests(SqliteTestsFixture fixture)
{
    private SqliteConnection Connection { get; init; } = fixture.Connection;

    [Fact]
    public void InnerJoin_FluentBuilder_ReturnsDataIfTrue()
    {
        var dt = Connection.Retrieve<Card>()
            .InnerJoin(e => e.DustCost,
                e => e.Id,
                r => r!.CardId)
            .ToList();
    }

    [Fact]
    public void EqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.Id == exId)
            .ToList();

        // Assert
        Assert.Single(weathers);
        Assert.Collection(weathers, weather => Assert.Equal(exId, weather.Id));
    }

    [Fact]
    public void NotEqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");
        const string exCountry = "Argentina";

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.Id == exId && w.Country != exCountry)
            .ToList();

        // Assert
        Assert.Empty(weathers);
    }

    [Fact]
    public void GreaterThan_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.TemperatureCelsius > exTemperatureCelsius)
            .ToList();

        // Assert
        Assert.Equal(9, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius > exTemperatureCelsius));
    }

    [Fact]
    public void GreaterThanOrEqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.TemperatureCelsius >= exTemperatureCelsius)
            .ToList();

        // Assert
        Assert.Equal(10, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius >= exTemperatureCelsius));
    }

    [Fact]
    public void LessThan_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.TemperatureCelsius < exTemperatureCelsius)
            .ToList();

        // Assert
        Assert.Equal(27, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius < exTemperatureCelsius));
    }

    [Fact]
    public void LessThanOrEqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.TemperatureCelsius <= exTemperatureCelsius)
            .ToList();

        // Assert
        Assert.Equal(32, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius <= exTemperatureCelsius));
    }

    [Fact]
    public void InOperator_FluentBuilder_ReturnsDataWhereValueExistsInList()
    {
        // Arrange
        string[] exCountries = ["Vietnam", "Canada"];

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => SqlFunctions.AnyIn(w.Country, exCountries))
            .ToList();

        // Assert
        Assert.Equal(20, weathers.Count);
        Assert.All(weathers, weather => Assert.Contains(weather.Country, exCountries));
    }

    [Fact]
    public void NotInOperator_FluentBuilder_ReturnsDataWhereValueExistsInList()
    {
        // Arrange
        string[] exCountries = ["Vietnam", "Canada"];

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => SqlFunctions.NotIn(w.Country, exCountries))
            .ToList();

        // Assert
        Assert.Equal(80, weathers.Count);
        Assert.DoesNotContain(weathers, weather => exCountries.Contains(weather.Country));
    }

    [Fact]
    public void BetweenOperator_FluentBuilder_ReturnsDataThatMatchValuesInRange()
    {
        // Arrange
        DateTime exDtBegin = new(2024, 5, 19);
        DateTime exDtEnd = new(2024, 5, 21);

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => SqlFunctions.InRange(w.LastUpdated, exDtBegin, exDtEnd))
            .ToList();

        // Assert
        Assert.Equal(10, weathers.Count);
        Assert.All(weathers, weather => Assert.InRange(weather.LastUpdated, exDtBegin, exDtEnd));
    }

    [Fact]
    public void OrOperator_FluentBuilder_ReturnsDataIfAnyOneConditionIsTrue()
    {
        // Arrange
        Guid[] exIds = [new("2DFA8730-2541-11EF-83FE-B1C709C359B7"), new("2DFA8731-2541-11EF-83FE-B1C709C359B7")];

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Select(w => new { w.Id, w.Country })
            .Where(w => w.Id == exIds[0] || w.Id == exIds[1])
            .ToList();

        // Assert
        Assert.Equal(2, weathers.Count);
        Assert.All(weathers, weather => Assert.Contains(weather.Id, exIds));
    }

    [Fact]
    public void SelectDistinct_FluentBuilder_ReturnsDataIfAnyOneConditionIsTrue()
    {
        // Arrange
        Guid[] exIds = [new("2DFA8730-2541-11EF-83FE-B1C709C359B7"), new("2DFA8731-2541-11EF-83FE-B1C709C359B7")];

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .SelectDistinct(w => new { w.Id, w.Country })
            .Where(w => w.Id == exIds[0] || w.Id == exIds[1])
            .ToList();

        // Assert
        Assert.Equal(2, weathers.Count);
        Assert.All(weathers, weather => Assert.Contains(weather.Id, exIds));
    }
}
