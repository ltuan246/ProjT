namespace KISS.QueryBuilder.Tests.UnitTests.Memory;

[Collection(nameof(MemoryDbTestsCollection))]
public sealed class FilterDefinitionBuilderTests(MemoryDbTestsFixture fixture)
{
    private SqliteConnection Connection { get; } = fixture.Connection;

    [Fact]
    public void Fetch_FluentBuilder_ReturnsExpectedWeathers()
    {
        // Act
        IList<MemoryDailyWeatherModel> weathers = Connection.Retrieve<MemoryDailyWeatherModel>()
            .From<MemoryDailyWeather>()
            .Select(w => new() { Id = w.Id })
            .ToList();

        // Assert
        Assert.Equal(5829, weathers.Count);
    }

    [Fact(Skip = "Doesn't work at the moment")]
    public void ConditionalFetching_FluentBuilder_ReturnsExpectedWeathers()
    {
        // Arrange
        MemoryDailyWeather exMemoryDailyWeather = new() { Id = new("61a05e1e-08c0-4a5d-9f30-ccd849a61017") };
        Guid exLocationId = new("23202fb3-a995-4e7e-a91e-eb192e2e9872");
        const double exMaxTempC = 2.7;
        DateTime exDt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        IList<MemoryDailyWeather> weathers = Connection.Retrieve<MemoryDailyWeather>()
            .From<MemoryDailyWeather>()
            .Where(w => CustomWeatherFilter(w, exLocationId))
            .Where(w => default)
            .Where(w => w.Id == exMemoryDailyWeather.Id)
            .Where(w => DateTime.Now.Ticks > 0)
            .Where(w => DateTime.Now > DateTime.MinValue)
            .Where(w => w.Date > exDt)
            .Where(w => new MemoryDailyWeather() { Id = new("61a05e1e-08c0-4a5d-9f30-ccd849a61017") } != null)
            // .Where(w => { var temp = w.MaxTempC; return temp == exMaxTempC; })
            .Where(w => w.MaxTempC == exMaxTempC)
            .Where(w => w.DateEpoch == 1740873600)
            .ToList();

        _ = weathers;

        // Assert
        // Assert.Equal(1, weathers.Count);
    }

    public bool CustomWeatherFilter(MemoryDailyWeather w, Guid locationId) => w.LocationId == locationId;
}
