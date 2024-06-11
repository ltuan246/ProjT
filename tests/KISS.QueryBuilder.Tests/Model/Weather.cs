namespace KISS.QueryBuilder.Tests.Model;

public class Weather
{
    [Name("id")] public required Guid Id { get; init; }

    [Name("country")] public required string Country { get; init; }
}