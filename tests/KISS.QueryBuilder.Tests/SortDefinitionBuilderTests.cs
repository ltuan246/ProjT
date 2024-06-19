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
        var query = WeatherRepository.Sort;
        var ascId = query.Ascending(t => t.Id);

        // Act
        List<Weather> weathers = WeatherRepository.Query(ascId);

        // Assert
        Assert.Collection(weathers,
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(new("2DFA8730-2541-11EF-83FE-B1C709C359B7"), weather.Id),
                    () => Assert.Equal("Argentina", weather.Country)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(new("2DFA8731-2541-11EF-83FE-B1C709C359B7"), weather.Id),
                    () => Assert.Equal("Iceland", weather.Country)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(new("2DFA8732-2541-11EF-83FE-B1C709C359B7"), weather.Id),
                    () => Assert.Equal("Iceland", weather.Country)));
    }

    [Fact]
    public void Descending()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");
        const string exCountry = "Argentina";

        var query = WeatherRepository.Sort;
        var descId = query.Descending(t => t.Id);

        // Act
        List<Weather> weathers = WeatherRepository.Query(descId);

        // Assert
        Assert.Collection(weathers,
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(exId, weather.Id),
                    () => Assert.Equal(exCountry, weather.Country)));
    }
}