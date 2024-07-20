namespace KISS.QueryBuilder.Tests.UnitTests;

[Collection(nameof(SqliteTestsCollection))]
public class ProjectionDefinitionBuilderTests(SqliteTestsFixture fixture)
{
    private SqliteConnection Connection { get; init; } = fixture.Connection;

    [Fact]
    public void ProjectionInclude()
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

        var projection = PredicateBuilder<Weather>.Select
            .Include(t => t.Country)
            .Include(t => t.WindMph)
            .Include(t => t.TemperatureCelsius)
            .Build();

        var limit = PredicateBuilder<Weather>.Fetch.Limit(3);

        // Act
        List<Weather> weathers = Connection.Gets<Weather>(projection, filters, sort, limit);

        // Assert
        Assert.Equal(3, weathers.Count);
        Assert.Collection(weathers,
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal("Argentina", weather.Country),
                    () => Assert.Equal(8.1, weather.WindMph),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius),
                    () => Assert.Null(weather.LocationName)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal("Argentina", weather.Country),
                    () => Assert.Equal(6.9, weather.WindMph),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius),
                    () => Assert.Null(weather.LocationName)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal("Iceland", weather.Country),
                    () => Assert.Equal(13.6, weather.WindMph),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius),
                    () => Assert.Null(weather.LocationName)));
    }

    [Fact]
    public void ProjectionExclude()
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

        var projection = PredicateBuilder<Weather>.Select
            .Exclude(t => t.Country)
            .Exclude(t => t.LocationName)
            .Build();

        var limit = PredicateBuilder<Weather>.Fetch.Limit(3);
        var offset = PredicateBuilder<Weather>.Fetch.Offset(3);

        // Act
        List<Weather> weathers = Connection.Gets<Weather>(projection, filters, sort, limit, offset);

        // Assert
        Assert.Equal(3, weathers.Count);
        Assert.Collection(weathers,
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Null(weather.Country),
                    () => Assert.Equal(11.9, weather.WindMph),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius),
                    () => Assert.Null(weather.LocationName)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Null(weather.Country),
                    () => Assert.Equal(5.6, weather.WindMph),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius),
                    () => Assert.Null(weather.LocationName)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Null(weather.Country),
                    () => Assert.Equal(4.3, weather.WindMph),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius),
                    () => Assert.Null(weather.LocationName)));
    }
}