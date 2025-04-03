namespace KISS.QueryBuilder.Tests.UnitTests.Memory;

[Collection(nameof(MemoryDbTestsCollection))]
public sealed class FilterDefinitionBuilderTests(MemoryDbTestsFixture fixture)
{
    private SqliteConnection Connection { get; } = fixture.Connection;

    [Fact]
    public void Fetch_FluentBuilder_ReturnsExpectedWeathers()
    {
        // Act
        IList<MemoryDailyWeather> weathers = Connection.Retrieve<MemoryDailyWeather>()
            .From<MemoryDailyWeather>()
            .ToList();

        // Assert
        Assert.Equal(5829, weathers.Count);
    }

    [Fact]
    public void ConditionalFetching_FluentBuilder_ReturnsExpectedWeathers()
    {
        // Arrange
        // MemoryDailyWeather exMemoryDailyWeather = new() { Id = new("61a05e1e-08c0-4a5d-9f30-ccd849a61017") };
        // Guid exLocationId = new("23202fb3-a995-4e7e-a91e-eb192e2e9872");
        // const double exMaxTempC = 2.7;

        // Act
        IList<MemoryDailyWeather> weathers = Connection.Retrieve<MemoryDailyWeather>()
            .From<MemoryDailyWeather>()
            // .Where(w => w.Id == exMemoryDailyWeather.Id)
            // .Where(w => w.LocationId == exLocationId)
            // .Where(w => w.MaxTempC == exMaxTempC)
            // .Where(w => w.DateEpoch == 1740873600)
            .ToList();

        // Assert
        // Assert.Equal(1, weathers.Count);
    }
}
