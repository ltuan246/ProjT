namespace KISS.QueryBuilder.Tests.UnitTests;

[Collection(nameof(SqliteTestsCollection))]
public class ProjectionDefinitionBuilderTests(SqliteTestsFixture fixture)
{
    private SqliteConnection Connection { get; init; } = fixture.Connection;
}
