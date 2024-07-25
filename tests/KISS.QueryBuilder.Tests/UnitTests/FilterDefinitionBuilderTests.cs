namespace KISS.QueryBuilder.Tests.UnitTests;

[Collection(nameof(SqliteTestsCollection))]
public sealed class FilterDefinitionBuilderTests(SqliteTestsFixture fixture)
{
    private SqliteConnection Connection { get; init; } = fixture.Connection;

    [Fact]
    public void WhenGettingAllWeathers_ThenAllWeathersReturn()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>();

        // Assert
        Assert.Equal(100, weathers.Count);
        Assert.Contains(weathers, weather => weather.Id == exId);
    }

    [Fact]
    public void QueryPredicateBuilder()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");
        const string exCountry = "Argentina";
        const int exTemperatureCelsius = 10;
        const double exWindMph = 19.2;

        var query = PredicateBuilder<Weather>.Filter;
        var idFilter = query.Eq(t => t.Id, exId);
        var countryFilter = query.Eq(t => t.Country, exCountry);

        var temperatureCelsiusFilter = query.Eq(t => t.TemperatureCelsius, exTemperatureCelsius);
        var windMphFilter = query.Eq(t => t.WindMph, exWindMph);

        var filter = query.Or(query.And(idFilter, countryFilter), query.And(temperatureCelsiusFilter, windMphFilter));

        var projection = PredicateBuilder<Weather>.Select
            .Include(t => t.Id)
            .Include(t => t.Country)
            .Include(t => t.TemperatureCelsius)
            .Include(t => t.WindMph)
            .Build();

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(projection, filter);

        // Assert
        Assert.Equal(2, weathers.Count);
        Assert.Collection(weathers,
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(exId, weather.Id),
                    () => Assert.Equal(exCountry, weather.Country)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius),
                    () => Assert.Equal(exWindMph, weather.WindMph)));
    }

    [Fact]
    public void Count()
    {
        // Act
        int count = Connection.Count<Weather>();

        // Assert
        Assert.Equal(100, count);
    }

    [Fact]
    public void Eq()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");

        var query = PredicateBuilder<Weather>.Filter;
        var idFilter = query.Eq(t => t.Id, exId);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(idFilter);

        // Assert
        Assert.Single(weathers);
        Assert.Collection(weathers, weather => Assert.Equal(exId, weather.Id));
    }

    [Fact]
    public void Ne()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");
        const string exCountry = "Argentina";

        var query = PredicateBuilder<Weather>.Filter;
        var idFilter = query.Eq(t => t.Id, exId);
        var countryFilter = query.Ne(t => t.Country, exCountry);
        var filter = query.And(idFilter, countryFilter);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(filter);

        // Assert
        Assert.Empty(weathers);
    }

    [Fact]
    public void Gt()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        var query = PredicateBuilder<Weather>.Filter;
        var temperatureFilter = query.Gt(t => t.TemperatureCelsius, exTemperatureCelsius);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(temperatureFilter);

        // Assert
        Assert.Equal(9, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius > exTemperatureCelsius));
    }

    [Fact]
    public void Gte()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        var query = PredicateBuilder<Weather>.Filter;
        var temperatureFilter = query.Gte(t => t.TemperatureCelsius, exTemperatureCelsius);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(temperatureFilter);

        // Assert
        Assert.Equal(10, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius >= exTemperatureCelsius));
    }

    [Fact]
    public void Lt()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        var query = PredicateBuilder<Weather>.Filter;
        var temperatureFilter = query.Lt(t => t.TemperatureCelsius, exTemperatureCelsius);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(temperatureFilter);

        // Assert
        Assert.Equal(27, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius < exTemperatureCelsius));
    }

    [Fact]
    public void Lte()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        var query = PredicateBuilder<Weather>.Filter;
        var temperatureFilter = query.Lte(t => t.TemperatureCelsius, exTemperatureCelsius);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(temperatureFilter);

        // Assert
        Assert.Equal(32, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius <= exTemperatureCelsius));
    }

    [Fact]
    public void AnyIn()
    {
        // Arrange
        string[] exCountries = ["Vietnam", "Canada"];

        var query = PredicateBuilder<Weather>.Filter;
        var countryFilter = query.AnyIn(t => t.Country, exCountries);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(countryFilter);

        // Assert
        Assert.Equal(20, weathers.Count);
        Assert.All(weathers, weather => Assert.Contains(weather.Country, exCountries));
    }

    [Fact]
    public void NotIn()
    {
        // Arrange
        string[] exCountries = ["Argentina", "Iceland", "Australia"];

        var query = PredicateBuilder<Weather>.Filter;
        var countryFilter = query.NotIn(t => t.Country, exCountries);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(countryFilter);

        // Assert
        Assert.Equal(63, weathers.Count);
        Assert.DoesNotContain(weathers, weather => exCountries.Contains(weather.Country));
    }

    [Fact]
    public void Within()
    {
        // Arrange
        DateTime exDtBegin = new(2024, 5, 19);
        DateTime exDtEnd = new(2024, 5, 21);

        var query = PredicateBuilder<Weather>.Filter;
        var rangeFilter = query.Within(t => t.LastUpdated, exDtBegin, exDtEnd);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(rangeFilter);

        // Assert
        Assert.Equal(10, weathers.Count);
        Assert.All(weathers, weather => Assert.InRange(weather.LastUpdated, exDtBegin, exDtEnd));
    }

    [Fact]
    public void And()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");
        const string exCountry = "Argentina";

        var query = PredicateBuilder<Weather>.Filter;
        var idFilter = query.Eq(t => t.Id, exId);
        var countryFilter = query.Eq(t => t.Country, exCountry);
        var filter = query.And(idFilter, countryFilter);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(filter);

        // Assert
        Assert.Single(weathers);
        Assert.Collection(weathers,
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(exId, weather.Id),
                    () => Assert.Equal(exCountry, weather.Country)));
    }

    [Fact]
    public void Or()
    {
        // Arrange
        Guid[] exIds = [new("2DFA8730-2541-11EF-83FE-B1C709C359B7"), new("2DFA8731-2541-11EF-83FE-B1C709C359B7")];

        var query = PredicateBuilder<Weather>.Filter;
        var idFilter1 = query.Eq(t => t.Id, exIds[0]);
        var idFilter2 = query.Eq(t => t.Id, exIds[1]);
        var filter = query.Or(idFilter1, idFilter2);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(filter);

        // Assert
        Assert.Equal(2, weathers.Count);
        Assert.All(weathers, weather => Assert.Contains(weather.Id, exIds));
    }

    [Fact]
    public void AndWithOr()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");
        const string exCountry = "Argentina";
        const int exTemperatureCelsius = 10;
        const double exWindMph = 19.2;

        var query = PredicateBuilder<Weather>.Filter;
        var idFilter = query.Eq(t => t.Id, exId);
        var countryFilter = query.Eq(t => t.Country, exCountry);

        var temperatureCelsiusFilter = query.Eq(t => t.TemperatureCelsius, exTemperatureCelsius);
        var windMphFilter = query.Eq(t => t.WindMph, exWindMph);

        var filter = query.Or(query.And(idFilter, countryFilter), query.And(temperatureCelsiusFilter, windMphFilter));

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(filter);

        // Assert
        Assert.Equal(2, weathers.Count);
        Assert.Collection(weathers,
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(exId, weather.Id),
                    () => Assert.Equal(exCountry, weather.Country)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius),
                    () => Assert.Equal(exWindMph, weather.WindMph)));
    }

    [Fact]
    public void GivenInvalidInput_WhenBuildQuery_ThenNotSupportedReturns()
    {
        // Arrange
        var builder = PredicateBuilder<Weather>.Filter;

        // Assert
        Assert.Throws<NotSupportedException>(() => builder.Eq(t => "", "a"));
    }
}