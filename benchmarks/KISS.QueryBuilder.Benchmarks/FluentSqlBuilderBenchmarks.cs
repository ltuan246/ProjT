namespace KISS.QueryBuilder.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class FluentSqlBuilderBenchmarks
{
    private SqliteConnection Connection { get; } = new();

    // Setup method to initialize the connection before benchmarks
    [GlobalSetup]
    public void Setup()
    {
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "weather.db");
        string connectionString = $"DataSource={dbPath};Mode=ReadWrite;Cache=Shared";
        Connection.ConnectionString = connectionString;
        Connection.Open();
    }

    // Cleanup method to dispose of the connection after benchmarks
    [GlobalCleanup]
    public void Cleanup()
        => Connection.Dispose();

    [Benchmark]
    public void EqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const string exId = "23202fb3-a995-4e7e-a91e-eb192e2e9872";

        // Act
        _ = Connection.Retrieve<Location>()
            .From<Location>()
            .Where(w => w.Id == exId)
            .ToList();
    }

    [Benchmark]
    public void NotEqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const string exId = "23202fb3-a995-4e7e-a91e-eb192e2e9872", exTzId = "Europe/Andorra";

        // Act
        _ = Connection.Retrieve<Location>()
            .From<Location>()
            .Where(w => w.Id == exId)
            .Where(w => w.TzId != exTzId)
            .ToList();
    }

    [Benchmark]
    public void GreaterThan_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        // Act
        _ = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.AvgTempC > exTemperatureCelsius)
            .ToList();
    }

    [Benchmark]
    public void GreaterThanOrEqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        // Act
        _ = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.AvgTempC >= exTemperatureCelsius)
            .ToList();
    }

    [Benchmark]
    public void LessThan_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        // Act
        _ = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.AvgTempC < exTemperatureCelsius)
            .ToList();
    }

    [Benchmark]
    public void LessThanOrEqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        // Act
        _ = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.AvgTempC <= exTemperatureCelsius)
            .ToList();
    }

    [Benchmark]
    public void InOperator_FluentBuilder_ReturnsDataWhereValueExistsInList()
    {
        // Arrange
        string[] exConditionTexts = ["Sunny", "Cloudy"];

        // Act
        _ = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => SqlFunctions.AnyIn(w.ConditionText, exConditionTexts))
            .ToList();
    }

    [Benchmark]
    public void NotInOperator_FluentBuilder_ReturnsDataWhereValueExistsInList()
    {
        // Arrange
        string[] exConditionTexts = ["Sunny", "Cloudy"];

        // Act
        _ = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => SqlFunctions.NotIn(w.ConditionText, exConditionTexts))
            .ToList();
    }

    [Benchmark]
    public void BetweenOperator_FluentBuilder_ReturnsDataThatMatchValuesInRange()
    {
        // Arrange
        DateTime exDateBegin = new(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime exDateEnd = new(2025, 3, 7, 0, 0, 0, DateTimeKind.Utc);
        long exDateBeginEpoch = EpochTime.GetIntDate(exDateBegin), exDateEndEpoch = EpochTime.GetIntDate(exDateEnd);

        // Act
        _ = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => SqlFunctions.InRange(w.DateEpoch, exDateBeginEpoch, exDateEndEpoch))
            .ToList();
    }

    [Benchmark]
    public void OrOperator_FluentBuilder_ReturnsDataIfAnyOneConditionIsTrue()
    {
        // Arrange
        string[] exIds =
            [new string("b804d8ae-791b-4c51-a164-e823146297d4"), new string("7489b710-5661-4068-b904-899e7f0df0b7")];

        // Act
        _ = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.Id == exIds[0] || w.Id == exIds[1])
            .Select(w => new DailyWeather
            {
                Id = w.Id,
                LocationId = w.LocationId,
                Date = w.Date,
                ConditionText = w.ConditionText,
                ConditionIcon = w.ConditionIcon
            })
            .ToList();
    }

    [Benchmark]
    public void SelectLimit_FluentBuilder_ReturnsDataIfAnyOneConditionIsTrue()
    {
        // Arrange
        string[] exIds =
            [new string("b804d8ae-791b-4c51-a164-e823146297d4"), new string("7489b710-5661-4068-b904-899e7f0df0b7")];

        // Act
        _ = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.Id == exIds[0] || w.Id == exIds[1])
            .Select(w => new DailyWeather
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
    }

    [Benchmark]
    public void SelectOffset_FluentBuilder_ReturnsDataIfAnyOneConditionIsTrue()
    {
        // Arrange
        string[] exIds =
            [new string("b804d8ae-791b-4c51-a164-e823146297d4"), new string("7489b710-5661-4068-b904-899e7f0df0b7")];

        // Act
        _ = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .Where(w => w.Id == exIds[0] || w.Id == exIds[1])
            .Select(w => new DailyWeather
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
    }

    [Benchmark]
    public void Join_FluentBuilder_ReturnsExpectedData()
    {
        // Arrange
        const string exId = "23202fb3-a995-4e7e-a91e-eb192e2e9872";

        // Act
        _ = Connection.Retrieve<WeatherModel>()
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
    }

    [Benchmark]
    public void GroupBy_FluentBuilder_ReturnsExpectedData()
    {
        // Act
        _ = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .GroupBy(c => c.LocationId)
            .ToDictionary();
    }

    [Benchmark]
    public void Having_FluentBuilder_ReturnsExpectedData()
    {
        // Arrange
        const double exTotalSnowCm = 20;

        // Act
        _ = Connection.Retrieve<DailyWeather>()
            .From<DailyWeather>()
            .GroupBy(c => c.LocationId)
            .Having(agg => agg.Sum(x => x.TotalSnowCm) > exTotalSnowCm)
            .SelectAggregate(agg => agg.Sum(x => x.TotalSnowCm), "TotalSnowCm")
            .ToDictionary();
    }
}
