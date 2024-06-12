namespace KISS.QueryBuilder.Tests.Model;

public class Weather
{
    [Name("id")] public required Guid Id { get; init; }

    [Name("country"), MaxLength(100)] public required string Country { get; init; }

    [Name("temperature_celsius")] public required float TemperatureCelsius { get; init; }
}