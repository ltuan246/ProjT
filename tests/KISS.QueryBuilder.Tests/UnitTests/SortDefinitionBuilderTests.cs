namespace KISS.QueryBuilder.Tests.UnitTests;

[Collection(nameof(SqliteTestsCollection))]
public class SortDefinitionBuilderTests(SqliteTestsFixture fixture)
{
    private SqliteConnection Connection { get; init; } = fixture.Connection;

    [Fact]
    public void WhenGettingAllWeathers_ThenAllWeathersReturn()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");

        // Act
        List<Weather> weathers = Connection.Gets<Weather>();

        // Assert
        Assert.Equal(100, weathers.Count);
        Assert.Contains(weathers, weather => weather.Id == exId);
    }

    [Fact]
    public void Ascending()
    {
        // Arrange
        const string exCountry = "Argentina";
        const int exTemperatureCelsius = 13;

        var ft = PredicateBuilder<Weather>.Filter;
        var countryFilter = ft.Eq(t => t.Country, exCountry);
        var temperatureCelsiusFilter = ft.Eq(t => t.TemperatureCelsius, exTemperatureCelsius);
        var filters = ft.And(countryFilter, temperatureCelsiusFilter);

        var sort = PredicateBuilder<Weather>.Sort
            .Ascending(t => t.Id)
            .Build();

        // Act
        List<Weather> weathers = Connection.Gets<Weather>(filters, sort);

        // Assert
        Assert.Equal(3, weathers.Count);
        Assert.Collection(weathers,
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(new("2DFA8740-2541-11EF-83FE-B1C709C359B7"), weather.Id),
                    () => Assert.Equal(exCountry, weather.Country),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(new("2DFA8749-2541-11EF-83FE-B1C709C359B7"), weather.Id),
                    () => Assert.Equal(exCountry, weather.Country),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(new("2DFA874A-2541-11EF-83FE-B1C709C359B7"), weather.Id),
                    () => Assert.Equal(exCountry, weather.Country),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius)));
    }

    [Fact]
    public void Descending()
    {
        // Arrange
        const string exCountry = "Argentina";
        const int exTemperatureCelsius = 13;

        var ft = PredicateBuilder<Weather>.Filter;
        var countryFilter = ft.Eq(t => t.Country, exCountry);
        var temperatureCelsiusFilter = ft.Eq(t => t.TemperatureCelsius, exTemperatureCelsius);
        var filters = ft.And(countryFilter, temperatureCelsiusFilter);

        var sort = PredicateBuilder<Weather>.Sort
            .Descending(t => t.Id)
            .Build();

        // Act
        List<Weather> weathers = Connection.Gets<Weather>(filters, sort);

        // Assert
        Assert.Equal(3, weathers.Count);
        Assert.Collection(weathers,
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(new("2DFA874A-2541-11EF-83FE-B1C709C359B7"), weather.Id),
                    () => Assert.Equal(exCountry, weather.Country),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(new("2DFA8749-2541-11EF-83FE-B1C709C359B7"), weather.Id),
                    () => Assert.Equal(exCountry, weather.Country),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(new("2DFA8740-2541-11EF-83FE-B1C709C359B7"), weather.Id),
                    () => Assert.Equal(exCountry, weather.Country),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius)));
    }

    [Fact]
    public void MultipleColumnOrdering()
    {
        // Arrange
        const int exTemperatureCelsius = 8;
        const int exWindMph = 15;

        var ft = PredicateBuilder<Weather>.Filter;
        var temperatureCelsiusFilter = ft.Eq(t => t.TemperatureCelsius, exTemperatureCelsius);
        var windMphFilter = ft.Lt(t => t.WindMph, exWindMph);
        var filters = ft.And(temperatureCelsiusFilter, windMphFilter);

        var sort = PredicateBuilder<Weather>.Sort
            .Ascending(t => t.Country)
            .Descending(t => t.WindMph)
            .Build();

        // Act
        List<Weather> weathers = Connection.Gets<Weather>(filters, sort);

        // Assert
        Assert.Equal(6, weathers.Count);
        Assert.Collection(weathers,
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal("Argentina", weather.Country),
                    () => Assert.Equal(8.1, weather.WindMph),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal("Argentina", weather.Country),
                    () => Assert.Equal(6.9, weather.WindMph),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal("Iceland", weather.Country),
                    () => Assert.Equal(13.6, weather.WindMph),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal("Iceland", weather.Country),
                    () => Assert.Equal(11.9, weather.WindMph),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal("New Zealand", weather.Country),
                    () => Assert.Equal(5.6, weather.WindMph),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal("New Zealand", weather.Country),
                    () => Assert.Equal(4.3, weather.WindMph),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius)));
    }
}