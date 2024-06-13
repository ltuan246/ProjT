namespace KISS.QueryBuilder.Tests;

public class FilterDefinitionBuilderTests : IDisposable
{
    private SqliteConnection Connection { get; init; }
    private ApplicationDbContext Context { get; init; }
    private GenericRepository<Weather> WeatherRepository { get; init; }

    public FilterDefinitionBuilderTests()
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
    public void Count()
    {
        // Act
        int count = WeatherRepository.Count();

        // Assert
        Assert.Equal(100, count);
    }

    [Fact]
    public void Eq()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");

        var query = WeatherRepository.Filter;
        var idFilter = query.Eq(t => t.Id, exId);

        // Act
        List<Weather> weathers = WeatherRepository.Query(idFilter);

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

        var query = WeatherRepository.Filter;
        var idFilter = query.Eq(t => t.Id, exId);
        var countryFilter = query.Ne(t => t.Country, exCountry);
        var filter = query.And(idFilter, countryFilter);

        // Act
        List<Weather> weathers = WeatherRepository.Query(filter);

        // Assert
        Assert.Empty(weathers);
    }

    [Fact]
    public void Gt()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        var query = WeatherRepository.Filter;
        var temperatureFilter = query.Gt(t => t.TemperatureCelsius, exTemperatureCelsius);

        // Act
        List<Weather> weathers = WeatherRepository.Query(temperatureFilter);

        // Assert
        Assert.Equal(9, weathers.Count());
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius > exTemperatureCelsius));
    }

    [Fact]
    public void Gte()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        var query = WeatherRepository.Filter;
        var temperatureFilter = query.Gte(t => t.TemperatureCelsius, exTemperatureCelsius);

        // Act
        List<Weather> weathers = WeatherRepository.Query(temperatureFilter);

        // Assert
        Assert.Equal(10, weathers.Count());
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius >= exTemperatureCelsius));
    }

    [Fact]
    public void Lt()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        var query = WeatherRepository.Filter;
        var temperatureFilter = query.Lt(t => t.TemperatureCelsius, exTemperatureCelsius);

        // Act
        List<Weather> weathers = WeatherRepository.Query(temperatureFilter);

        // Assert
        Assert.Equal(27, weathers.Count());
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius < exTemperatureCelsius));
    }

    [Fact]
    public void Lte()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        var query = WeatherRepository.Filter;
        var temperatureFilter = query.Lte(t => t.TemperatureCelsius, exTemperatureCelsius);

        // Act
        List<Weather> weathers = WeatherRepository.Query(temperatureFilter);

        // Assert
        Assert.Equal(32, weathers.Count());
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius <= exTemperatureCelsius));
    }

    [Fact]
    public void AnyIn()
    {
        // Arrange
        string[] exCountries = ["Vietnam", "Canada"];

        var query = WeatherRepository.Filter;
        var countryFilter = query.AnyIn(t => t.Country, exCountries);

        // Act
        List<Weather> weathers = WeatherRepository.Query(countryFilter);

        // Assert
        Assert.Equal(20, weathers.Count());
        Assert.All(weathers, weather => Assert.Contains(weather.Country, exCountries));
    }

    [Fact]
    public void NotIn()
    {
        // Arrange
        string[] exCountries = ["Argentina", "Iceland", "Australia"];

        var query = WeatherRepository.Filter;
        var countryFilter = query.NotIn(t => t.Country, exCountries);

        // Act
        List<Weather> weathers = WeatherRepository.Query(countryFilter);

        // Assert
        Assert.Equal(63, weathers.Count());
        Assert.DoesNotContain(weathers, weather => exCountries.Contains(weather.Country));
    }

    [Fact]
    public void And()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");
        const string exCountry = "Argentina";

        var query = WeatherRepository.Filter;
        var idFilter = query.Eq(t => t.Id, exId);
        var countryFilter = query.Eq(t => t.Country, exCountry);
        var filter = query.And(idFilter, countryFilter);

        // Act
        List<Weather> weathers = WeatherRepository.Query(filter);

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

        var query = WeatherRepository.Filter;
        var idFilter1 = query.Eq(t => t.Id, exIds[0]);
        var idFilter2 = query.Eq(t => t.Id, exIds[1]);
        var filter = query.Or(idFilter1, idFilter2);

        // Act
        List<Weather> weathers = WeatherRepository.Query(filter);

        // Assert
        Assert.Equal(2, weathers.Count());
        Assert.All(weathers, weather => Assert.Contains(weather.Id, exIds));
    }

    [Fact]
    public void GivenInvalidInput_WhenBuildQuery_ThenNotSupportedReturns()
    {
        // Arrange
        var builder = WeatherRepository.Filter;
        var filter = builder.Eq(t => "", "a");

        // Assert
        Assert.Throws<NotSupportedException>(() => filter.Render());
    }
}