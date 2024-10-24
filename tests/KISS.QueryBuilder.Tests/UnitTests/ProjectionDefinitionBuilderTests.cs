namespace KISS.QueryBuilder.Tests.UnitTests;

[Collection(nameof(SqliteTestsCollection))]
public class ProjectionDefinitionBuilderTests(SqliteTestsFixture fixture)
{
    private SqliteConnection Connection { get; init; } = fixture.Connection;

    [Fact(Skip = "Doesn't work at the moment")]
    public void LimitClause_FluentBuilder_ReturnsSpecifyTheNumberOfRecordsInTheResultSet()
    {
        // Arrange
        const double exTemperatureCelsius = 8;
        const int exWindMph = 15;

        // Act
        IList<Weather> weathers = Connection.Retrieve<Weather>()
            .Where(w => w.TemperatureCelsius == exTemperatureCelsius && w.WindMph < exWindMph)
            .OrderBy(w => w.Id)
            .Limit(3)
            .Offset(3)
            .ToList();

        // Assert
        Assert.Equal(3, weathers.Count);
    }
}
