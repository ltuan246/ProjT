namespace KISS.QueryBuilder.Tests.Sqlite;

[CollectionDefinition(nameof(SqliteTestsCollection))]
public class SqliteTestsCollection : ICollectionFixture<SqliteTestsFixture>;

[CollectionDefinition(nameof(MemoryDbTestsCollection))]
public class MemoryDbTestsCollection : ICollectionFixture<MemoryDbTestsFixture>;
