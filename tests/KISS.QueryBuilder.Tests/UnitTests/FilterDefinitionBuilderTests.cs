namespace KISS.QueryBuilder.Tests.UnitTests;

[Collection(nameof(SqliteTestsCollection))]
public sealed class FilterDefinitionBuilderTests(SqliteTestsFixture fixture)
{
    private SqliteConnection Connection { get; init; } = fixture.Connection;

    [Fact]
    public void WhenGettingAllWeathers_ThenAllWeathersReturn()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>();

        // Assert
        Assert.Equal(100, weathers.Count);
        Assert.Contains(weathers, weather => weather.Id == exId);
    }

    [Fact]
    public void WhenGettingAllCards_ThenAllWeathersReturn()
    {
        // Act
        IList<Card> cards = Connection.Gets<Card>();
        IList<DustCost> dustCosts = Connection.Gets<DustCost>();

        // Assert
        Assert.Equal(2819, cards.Count);
        Assert.Equal(4040, dustCosts.Count);
    }

    [Fact]
    public void Integration_QueryBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");
        const string exCountry = "Argentina";
        const int exTemperatureCelsius = 10;
        const double exWindMph = 19.2;

        var query = PredicateBuilder<Weather>.Filter;
        var idFilter = query.Eq(t => t.Id, exId);
        var countryFilter = query.Eq(t => t.Country, exCountry);

        var temperatureCelsiusFilter = query.Eq(t => t.TemperatureCelsius, exTemperatureCelsius);
        var windMphFilter = query.Eq(t => t.WindMph, exWindMph);

        var filter = query.Or(query.And(idFilter, countryFilter), query.And(temperatureCelsiusFilter, windMphFilter));

        var projection = PredicateBuilder<Weather>.Select
            .Include(t => t.Id)
            .Include(t => t.Country)
            .Include(t => t.TemperatureCelsius)
            .Include(t => t.WindMph)
            .Build();

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(projection, filter);

        // Assert
        Assert.Equal(2, weathers.Count);
        Assert.Collection(weathers,
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(exId, weather.Id),
                    () => Assert.Equal(exCountry, weather.Country)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius),
                    () => Assert.Equal(exWindMph, weather.WindMph)));
    }

    [Fact]
    public void Count_QueryBuilder_ReturnsTheNumberOfRecordsInTable()
    {
        // Act
        int count = Connection.Count<Weather>();

        // Assert
        Assert.Equal(100, count);
    }

    [Fact]
    public void EqualTo_QueryBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");

        var query = PredicateBuilder<Weather>.Filter;
        var idFilter = query.Eq(t => t.Id, exId);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(idFilter);

        // Assert
        Assert.Single(weathers);
        Assert.Collection(weathers, weather => Assert.Equal(exId, weather.Id));
    }

    [Fact]
    public void EqualTo_FluentBuilder_ReturnsDataIfTrue()
    {   
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");

        // Act
        var ret = Connection.Retrieving<Weather>()
            .Select((Weather w) => new { w.Id, w.Country })
            .Where();

        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.Id == exId)
            .ToList();

        // Assert
        Assert.Single(weathers);
        Assert.Collection(weathers, weather => Assert.Equal(exId, weather.Id));
    }

    [Fact]
    public void NotEqualTo_QueryBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");
        const string exCountry = "Argentina";

        var query = PredicateBuilder<Weather>.Filter;
        var idFilter = query.Eq(t => t.Id, exId);
        var countryFilter = query.Ne(t => t.Country, exCountry);
        var filter = query.And(idFilter, countryFilter);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(filter);

        // Assert
        Assert.Empty(weathers);
    }

    [Fact]
    public void NotEqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");
        const string exCountry = "Argentina";

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.Id == exId && w.Country != exCountry)
            .ToList();

        // Assert
        Assert.Empty(weathers);
    }

    [Fact]
    public void GreaterThan_QueryBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        var query = PredicateBuilder<Weather>.Filter;
        var temperatureFilter = query.Gt(t => t.TemperatureCelsius, exTemperatureCelsius);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(temperatureFilter);

        // Assert
        Assert.Equal(9, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius > exTemperatureCelsius));
    }

    [Fact]
    public void GreaterThan_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.TemperatureCelsius > exTemperatureCelsius)
            .ToList();

        // Assert
        Assert.Equal(9, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius > exTemperatureCelsius));
    }

    [Fact]
    public void GreaterThanOrEqualTo_QueryBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        var query = PredicateBuilder<Weather>.Filter;
        var temperatureFilter = query.Gte(t => t.TemperatureCelsius, exTemperatureCelsius);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(temperatureFilter);

        // Assert
        Assert.Equal(10, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius >= exTemperatureCelsius));
    }

    [Fact]
    public void GreaterThanOrEqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 29;

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.TemperatureCelsius >= exTemperatureCelsius)
            .ToList();

        // Assert
        Assert.Equal(10, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius >= exTemperatureCelsius));
    }

    [Fact]
    public void LessThan_QueryBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        var query = PredicateBuilder<Weather>.Filter;
        var temperatureFilter = query.Lt(t => t.TemperatureCelsius, exTemperatureCelsius);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(temperatureFilter);

        // Assert
        Assert.Equal(27, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius < exTemperatureCelsius));
    }

    [Fact]
    public void LessThan_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.TemperatureCelsius < exTemperatureCelsius)
            .ToList();

        // Assert
        Assert.Equal(27, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius < exTemperatureCelsius));
    }

    [Fact]
    public void LessThanOrEqualTo_QueryBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        var query = PredicateBuilder<Weather>.Filter;
        var temperatureFilter = query.Lte(t => t.TemperatureCelsius, exTemperatureCelsius);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(temperatureFilter);

        // Assert
        Assert.Equal(32, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius <= exTemperatureCelsius));
    }

    [Fact]
    public void LessThanOrEqualTo_FluentBuilder_ReturnsDataIfTrue()
    {
        // Arrange
        const float exTemperatureCelsius = 10;

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.TemperatureCelsius <= exTemperatureCelsius)
            .ToList();

        // Assert
        Assert.Equal(32, weathers.Count);
        Assert.All(weathers, weather => Assert.True(weather.TemperatureCelsius <= exTemperatureCelsius));
    }

    [Fact]
    public void InOperator_QueryBuilder_ReturnsDataWhereValueExistsInList()
    {
        // Arrange
        string[] exCountries = ["Vietnam", "Canada"];

        var query = PredicateBuilder<Weather>.Filter;
        var countryFilter = query.AnyIn(t => t.Country, exCountries);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(countryFilter);

        // Assert
        Assert.Equal(20, weathers.Count);
        Assert.All(weathers, weather => Assert.Contains(weather.Country, exCountries));
    }

    [Fact]
    public void InOperator_FluentBuilder_ReturnsDataWhereValueExistsInList()
    {
        // Arrange
        string[] exCountries = ["Vietnam", "Canada"];

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => SqlExpression.AnyIn(w.Country, exCountries))
            .ToList();

        // Assert
        Assert.Equal(20, weathers.Count);
        Assert.All(weathers, weather => Assert.Contains(weather.Country, exCountries));
    }

    [Fact]
    public void NotInOperator_QueryBuilder_ReturnsAllDataExceptTheExcludedValuesInList()
    {
        // Arrange
        string[] exCountries = ["Argentina", "Iceland", "Australia"];

        var query = PredicateBuilder<Weather>.Filter;
        var countryFilter = query.NotIn(t => t.Country, exCountries);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(countryFilter);

        // Assert
        Assert.Equal(63, weathers.Count);
        Assert.DoesNotContain(weathers, weather => exCountries.Contains(weather.Country));
    }

    [Fact]
    public void NotInOperator_FluentBuilder_ReturnsDataWhereValueExistsInList()
    {
        // Arrange
        string[] exCountries = ["Vietnam", "Canada"];

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => SqlExpression.NotIn(w.Country, exCountries))
            .ToList();

        // Assert
        Assert.Equal(80, weathers.Count);
        Assert.DoesNotContain(weathers, weather => exCountries.Contains(weather.Country));
    }

    [Fact]
    public void BetweenOperator_QueryBuilder_ReturnsDataThatMatchValuesInRange()
    {
        // Arrange
        DateTime exDtBegin = new(2024, 5, 19);
        DateTime exDtEnd = new(2024, 5, 21);

        var query = PredicateBuilder<Weather>.Filter;
        var rangeFilter = query.Within(t => t.LastUpdated, exDtBegin, exDtEnd);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(rangeFilter);

        // Assert
        Assert.Equal(10, weathers.Count);
        Assert.All(weathers, weather => Assert.InRange(weather.LastUpdated, exDtBegin, exDtEnd));
    }

    [Fact]
    public void BetweenOperator_FluentBuilder_ReturnsDataThatMatchValuesInRange()
    {
        // Arrange
        DateTime exDtBegin = new(2024, 5, 19);
        DateTime exDtEnd = new(2024, 5, 21);

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => SqlExpression.InRange(w.LastUpdated, exDtBegin, exDtEnd))
            .ToList();

        // Assert
        Assert.Equal(10, weathers.Count);
        Assert.All(weathers, weather => Assert.InRange(weather.LastUpdated, exDtBegin, exDtEnd));
    }

    [Fact]
    public void AndOperator_QueryBuilder_ReturnsDataIfAllConditionsAreTrue()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");
        const string exCountry = "Argentina";

        var query = PredicateBuilder<Weather>.Filter;
        var idFilter = query.Eq(t => t.Id, exId);
        var countryFilter = query.Eq(t => t.Country, exCountry);
        var filter = query.And(idFilter, countryFilter);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(filter);

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
    public void OrOperator_QueryBuilder_ReturnsDataIfAnyOneConditionIsTrue()
    {
        // Arrange
        Guid[] exIds = [new("2DFA8730-2541-11EF-83FE-B1C709C359B7"), new("2DFA8731-2541-11EF-83FE-B1C709C359B7")];

        var query = PredicateBuilder<Weather>.Filter;
        var idFilter1 = query.Eq(t => t.Id, exIds[0]);
        var idFilter2 = query.Eq(t => t.Id, exIds[1]);
        var filter = query.Or(idFilter1, idFilter2);

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(filter);

        // Assert
        Assert.Equal(2, weathers.Count);
        Assert.All(weathers, weather => Assert.Contains(weather.Id, exIds));
    }

    [Fact]
    public void OrOperator_FluentBuilder_ReturnsDataIfAnyOneConditionIsTrue()
    {
        // Arrange
        Guid[] exIds = [new("2DFA8730-2541-11EF-83FE-B1C709C359B7"), new("2DFA8731-2541-11EF-83FE-B1C709C359B7")];

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Select(w => new { w.Id, w.Country })
            .Where(w => w.Id == exIds[0] || w.Id == exIds[1])
            .ToList();

        // Assert
        Assert.Equal(2, weathers.Count);
        Assert.All(weathers, weather => Assert.Contains(weather.Id, exIds));
    }

    [Fact]
    public void SelectDistinct_FluentBuilder_ReturnsDataIfAnyOneConditionIsTrue()
    {
        // Arrange
        Guid[] exIds = [new("2DFA8730-2541-11EF-83FE-B1C709C359B7"), new("2DFA8731-2541-11EF-83FE-B1C709C359B7")];

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .SelectDistinct(w => new { w.Id, w.Country })
            .Where(w => w.Id == exIds[0] || w.Id == exIds[1])
            .ToList();

        // Assert
        Assert.Equal(2, weathers.Count);
        Assert.All(weathers, weather => Assert.Contains(weather.Id, exIds));
    }

    [Fact]
    public void AndOrOperator_QueryBuilder_ReturnsDataIfAnyOneOperatorIsTrue()
    {
        // Arrange
        Guid exId = new("2DFA8730-2541-11EF-83FE-B1C709C359B7");
        const string exCountry = "Argentina";
        const int exTemperatureCelsius = 10;
        const double exWindMph = 19.2;

        var query = PredicateBuilder<Weather>.Filter;
        var idFilter = query.Eq(t => t.Id, exId);
        var countryFilter = query.Eq(t => t.Country, exCountry);

        var temperatureCelsiusFilter = query.Eq(t => t.TemperatureCelsius, exTemperatureCelsius);
        var windMphFilter = query.Eq(t => t.WindMph, exWindMph);

        var filter = query.Or(query.And(idFilter, countryFilter), query.And(temperatureCelsiusFilter, windMphFilter));

        // Act
        IList<Weather> weathers = Connection.Gets<Weather>(filter);

        // Assert
        Assert.Equal(2, weathers.Count);
        Assert.Collection(weathers,
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(exId, weather.Id),
                    () => Assert.Equal(exCountry, weather.Country)),
            weather =>
                Assert.Multiple(
                    () => Assert.NotNull(weather),
                    () => Assert.Equal(exTemperatureCelsius, weather.TemperatureCelsius),
                    () => Assert.Equal(exWindMph, weather.WindMph)));
    }

    [Fact]
    public void QueryBuilder_InvalidInput_ThrowsNotSupportedException()
    {
        // Arrange
        var builder = PredicateBuilder<Weather>.Filter;

        // Assert
        Assert.Throws<NotSupportedException>(() => builder.Eq(t => "", "a"));
    }
}
