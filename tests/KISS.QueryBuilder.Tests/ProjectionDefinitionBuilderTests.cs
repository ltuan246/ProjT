namespace KISS.QueryBuilder.Tests;

public class ProjectionDefinitionBuilderTests : IDisposable
{
    private SqliteConnection Connection { get; init; }
    private ApplicationDbContext Context { get; init; }
    private GenericRepository<Weather> WeatherRepository { get; init; }

    public ProjectionDefinitionBuilderTests()
    {
        // https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/dapper-limitations
        SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
        SqlMapper.AddTypeHandler(new GuidHandler());
        SqlMapper.AddTypeHandler(new TimeSpanHandler());

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
    public void ProjectionInclude()
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

        var projection = WeatherRepository.Projection;
        var countryColumn = projection.Include(t => t.Country);
        var windMphColumn = projection.Include(t => t.WindMph);
        var temperatureCelsiusColumn = projection.Include(t => t.TemperatureCelsius);
        var limit = projection.Slice(3);
        var specificColumns = projection.Combine(countryColumn, windMphColumn, temperatureCelsiusColumn, limit);

        // Act
        List<Weather> weathers = WeatherRepository.Query(specificColumns, filters, sorts);

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
        var offset = PredicateBuilder<Weather>.Offset.Offset(3);

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