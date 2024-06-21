namespace KISS.QueryBuilder.Tests;

public class SortDefinitionBuilderTests : IDisposable
{
    private SqliteConnection Connection { get; init; }
    private ApplicationDbContext Context { get; init; }
    private GenericRepository<Weather> WeatherRepository { get; init; }

    public SortDefinitionBuilderTests()
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/dapper-limitations
        SqlMapper.AddTypeHandler(new GuidHandler());

        const string connectionString = "datasource=:memory:";
        Connection = new SqliteConnection(connectionString);
        Connection.Open();

        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(Connection)
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();

        WeatherRepository = new(Context);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Connection.Close();
        Connection.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void WhenGettingAllWeathers_ThenAllWeathersReturn()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");

        // Act
        List<Weather> weathers = WeatherRepository.GetList();

        // Assert
        Assert.Equal(100, weathers.Count());
        Assert.Contains(weathers, weather => weather.Id == exId);
    }

    [Fact]
    public void Ascending()
    {
        // Arrange
        const string exCountry = "Argentina";
        const int exTemperatureCelsius = 13;

        var ft = WeatherRepository.Filter;
        var countryFilter = ft.Eq(t => t.Country, exCountry);
        var temperatureCelsiusFilter = ft.Eq(t => t.TemperatureCelsius, exTemperatureCelsius);
        var filters = ft.And(countryFilter, temperatureCelsiusFilter);

        var sort = WeatherRepository.Sort;
        var ascId = sort.Ascending(t => t.Id);

        // Act
        List<Weather> weathers = WeatherRepository.Query(filters, ascId);

        // Assert
        Assert.Equal(3, weathers.Count());
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

        var ft = WeatherRepository.Filter;
        var countryFilter = ft.Eq(t => t.Country, exCountry);
        var temperatureCelsiusFilter = ft.Eq(t => t.TemperatureCelsius, exTemperatureCelsius);
        var filters = ft.And(countryFilter, temperatureCelsiusFilter);

        var sort = WeatherRepository.Sort;
        var descId = sort.Descending(t => t.Id);

        // Act
        List<Weather> weathers = WeatherRepository.Query(filters, descId);

        // Assert
        Assert.Equal(3, weathers.Count());
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

        var ft = WeatherRepository.Filter;
        var temperatureCelsiusFilter = ft.Eq(t => t.TemperatureCelsius, exTemperatureCelsius);
        var windMphFilter = ft.Lt(t => t.WindMph, exWindMph);
        var filters = ft.And(temperatureCelsiusFilter, windMphFilter);

        var sort = WeatherRepository.Sort;
        var ascCountry = sort.Ascending(t => t.Country);
        var descWindMph = sort.Descending(t => t.WindMph);
        var sorts = sort.Combine(ascCountry, descWindMph);

        // Act
        List<Weather> weathers = WeatherRepository.Query(filters, sorts);

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