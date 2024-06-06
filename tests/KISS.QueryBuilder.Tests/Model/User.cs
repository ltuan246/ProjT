namespace KISS.QueryBuilder.Tests.Model;

public sealed record User
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
}