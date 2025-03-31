namespace KISS.QueryBuilder.Tests.UnitTests;

[Collection(nameof(SqliteTestsCollection))]
public sealed class FilterDefinitionBuilderTests(SqliteTestsFixture fixture)
{
    private SqliteConnection Connection { get; init; } = fixture.Connection;

    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 201)]
    public void Fetch_FluentBuilder_ReturnsExpectedCards(bool condition, int expected)
    {
        // Arrange
        const string exId = "23202fb3-a995-4e7e-a91e-eb192e2e9872";

        // Act
        var weathers = Connection.Retrieve<WeatherModel>()
            .From<Location>()
            .InnerJoin<Astronomy>( // Map one-to-one relationship
                e => e.Id,
                r => r.LocationId,
                e => e.Astro)
            .InnerJoin<DailyWeather>( // Map one-to-one relationship
                (Location e) => e.Id,
                r => r.LocationId,
                e => e.DailyWeathers)
            .Where(condition, (Location w) => w.Id == exId)
            .ToList();

        // Assert
        Assert.Equal(expected, weathers.Count);
    }

    [Fact]
    public void EqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const string exId = "23202fb3-a995-4e7e-a91e-eb192e2e9872", exTzId = "Europe/Andorra";
        const double exLatitude = 42.5, exLongitude = 1.517;

        // Act
        IList<Location> locations = Connection.Retrieve<Location>()
            .From<Location>()
            .Where(w => w.Id == exId)
            .ToList();

        // Assert
        Assert.Single(locations);
        Assert.Collection(locations,
            location =>
            {
                Assert.Equal(exId, location.Id);
                Assert.Equal(exLatitude, location.Latitude);
                Assert.Equal(exLongitude, location.Longitude);
                Assert.Equal(exTzId, location.TzId);
            });
    }

    [Fact]
    public void NotEqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const string exId = "23202fb3-a995-4e7e-a91e-eb192e2e9872", exTzId = "Europe/Andorra";

        // Act
        IList<Location> locations = Connection.Retrieve<Location>()
            .From<Location>()
            .Where(w => w.Id == exId)
            .Where(w => w.TzId != exTzId)
            .ToList();

        // Assert
        Assert.Empty(locations);
    }

    [Fact]
    public void GreaterThan_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        // Act
        IList<DailyWeather> weathers = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.AvgTempC > exTemperatureCelsius)
            .ToList();

        // Assert
        Assert.Equal(433, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.AvgTempC > exTemperatureCelsius));
    }

    [Fact]
    public void GreaterThanOrEqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        // Act
        IList<DailyWeather> weathers = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.AvgTempC >= exTemperatureCelsius)
            .ToList();

        // Assert
        Assert.Equal(441, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.AvgTempC >= exTemperatureCelsius));
    }

    [Fact]
    public void LessThan_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        // Act
        IList<DailyWeather> weathers = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.AvgTempC < exTemperatureCelsius)
            .ToList();

        // Assert
        Assert.Equal(1612, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.AvgTempC < exTemperatureCelsius));
    }

    [Fact]
    public void LessThanOrEqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        // Act
        IList<DailyWeather> weathers = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.AvgTempC <= exTemperatureCelsius)
            .ToList();

        // Assert
        Assert.Equal(1628, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.AvgTempC <= exTemperatureCelsius));
    }

    [Fact]
    public void InOperator_FluentBuilder_ReturnsDataWhereValueExistsInList()
    {
        // Arrange
        string[] exConditionTexts = ["Sunny", "Cloudy"];

        // Act
        IList<DailyWeather> weathers = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => SqlFunctions.AnyIn(w.ConditionText, exConditionTexts))
            .ToList();

        // Assert
        Assert.Equal(1556, weathers.Count);
        Assert.All(weathers, weather => Assert.Contains(weather.ConditionText, exConditionTexts));
    }

    [Fact]
    public void NotInOperator_FluentBuilder_ReturnsDataWhereValueExistsInList()
    {
        // Arrange
        string[] exConditionTexts = ["Sunny", "Cloudy"];

        // Act
        IList<DailyWeather> weathers = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => SqlFunctions.NotIn(w.ConditionText, exConditionTexts))
            .ToList();

        // Assert
        Assert.Equal(4273, weathers.Count);
        Assert.DoesNotContain(weathers, weather => exConditionTexts.Contains(weather.ConditionText));
    }

    [Fact]
    public void BetweenOperator_FluentBuilder_ReturnsDataThatMatchValuesInRange()
    {
        // Arrange
        DateTime exDateBegin = new(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime exDateEnd = new(2025, 3, 7, 0, 0, 0, DateTimeKind.Utc);
        long exDateBeginEpoch = EpochTime.GetIntDate(exDateBegin), exDateEndEpoch = EpochTime.GetIntDate(exDateEnd);

        // Act
        IList<DailyWeather> weathers = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => SqlFunctions.InRange(w.DateEpoch, exDateBeginEpoch, exDateEndEpoch))
            .ToList();

        // Assert
        Assert.Equal(1407, weathers.Count);
        Assert.All(weathers, weather => Assert.InRange(weather.DateEpoch, exDateBeginEpoch, exDateEndEpoch));
    }

    [Fact]
    public void OrOperator_FluentBuilder_ReturnsDataIfAnyOneConditionIsTrue()
    {
        // Arrange
        string[] exIds = [new("b804d8ae-791b-4c51-a164-e823146297d4"), new("7489b710-5661-4068-b904-899e7f0df0b7")];

        // Act
        IList<DailyWeather> weathers = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.Id == exIds[0] || w.Id == exIds[1])
            .Select(w => new()
            {
                Id = w.Id,
                LocationId = w.LocationId,
                Date = w.Date,
                ConditionText = w.ConditionText,
                ConditionIcon = w.ConditionIcon
            })
            .ToList();

        // Assert
        Assert.Equal(2, weathers.Count);
        Assert.All(weathers, weather => Assert.Contains(weather.Id, exIds));
    }

    [Fact]
    public void SelectLimit_FluentBuilder_ReturnsDataIfAnyOneConditionIsTrue()
    {
        // Arrange
        string[] exIds = [new("b804d8ae-791b-4c51-a164-e823146297d4"), new("7489b710-5661-4068-b904-899e7f0df0b7")];

        // Act
        IList<DailyWeather> weathers = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.Id == exIds[0] || w.Id == exIds[1])
            .Select(w => new()
            {
                Id = w.Id,
                LocationId = w.LocationId,
                Date = w.Date,
                ConditionText = w.ConditionText,
                ConditionIcon = w.ConditionIcon
            })
            .OrderBy(w => w.Date)
            .Limit(1)
            .ToList();

        // Assert
        Assert.Single(weathers);
        Assert.Collection(weathers,
            c =>
            {
                Assert.Equal(exIds[0], c.Id);
            });
    }

    [Fact]
    public void SelectOffset_FluentBuilder_ReturnsDataIfAnyOneConditionIsTrue()
    {
        // Arrange
        string[] exIds = [new("b804d8ae-791b-4c51-a164-e823146297d4"), new("7489b710-5661-4068-b904-899e7f0df0b7")];

        // Act
        IList<DailyWeather> weathers = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.Id == exIds[0] || w.Id == exIds[1])
            .Select(w => new()
            {
                Id = w.Id,
                LocationId = w.LocationId,
                Date = w.Date,
                ConditionText = w.ConditionText,
                ConditionIcon = w.ConditionIcon
            })
            .OrderBy(w => w.Date)
            .Limit(1)
            .Offset(1)
            .ToList();

        // Assert
        Assert.Single(weathers);
        Assert.Collection(weathers,
            c =>
            {
                Assert.Equal(exIds[1], c.Id);
            });
    }

    [Fact]
    public void Join_FluentBuilder_ReturnsExpectedCards()
    {
        // Arrange
        const string exId = "23202fb3-a995-4e7e-a91e-eb192e2e9872", exMoonPhase = "New Moon";

        // Act
        IList<WeatherModel> weathers = Connection.Retrieve<WeatherModel>()
            .From<Location>()
            .InnerJoin<Astronomy>( // Map one-to-one relationship
                e => e.Id,
                r => r.LocationId,
                e => e.Astro)
            .InnerJoin<DailyWeather>( // Map one-to-one relationship
                (Location e) => e.Id,
                r => r.LocationId,
                e => e.DailyWeathers)
            .Where((Location c) => c.Id == exId)
            .ToList();

        // Assert
        Assert.Single(weathers);
        Assert.Collection(weathers,
            c =>
            {
                Assert.Equal(exId, c.Id);

                Assert.NotNull(c.Astro);
                Assert.Equal(exMoonPhase, c.Astro.MoonPhase);

                Assert.NotNull(c.DailyWeathers);
                Assert.Equal(841, c.DailyWeathers.Count);
                Assert.Equal(1282, c.DailyWeathers[0].ConditionCode);
                Assert.Equal("Moderate or heavy snow with thunder", c.DailyWeathers[0].ConditionText);
            });
    }

    [Fact]
    public void GroupBy_FluentBuilder_ReturnsExpectedCards()
    {
        // Act
        var weathers = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .GroupBy(c => c.LocationId)
            .ToDictionary();

        Assert.Equal(201, weathers.Count);
        Assert.All(weathers, weather => Assert.Equal(29, weather.Value.Count));
    }

    [Fact]
    public void Having_FluentBuilder_ReturnsExpectedCards()
    {
        // Arrange
        const double exTotalSnowCm = 20;

        // Act
        var weathers = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .GroupBy(c => c.LocationId)
            .Having(agg => agg.Sum(x => x.TotalSnowCm) > exTotalSnowCm)
            .SelectAggregate(agg => agg.Sum(x => x.TotalSnowCm), "TotalSnowCm")
            .ToDictionary();

        Assert.Equal(10, weathers.Count);
        Assert.All(weathers, weather => Assert.True((double)weather.Key[1]! > exTotalSnowCm));
    }
}
