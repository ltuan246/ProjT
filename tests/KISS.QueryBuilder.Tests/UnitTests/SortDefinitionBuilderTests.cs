namespace KISS.QueryBuilder.Tests.UnitTests;

[Collection(nameof(SqliteTestsCollection))]
public class SortDefinitionBuilderTests(SqliteTestsFixture fixture)
{
    private SqliteConnection Connection { get; init; } = fixture.Connection;
}
