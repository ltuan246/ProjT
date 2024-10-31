namespace KISS.QueryBuilder.Tests.Model;

public class Weather : IEntityBuilder
{
    [Name("id")] public required Guid Id { get; init; }

    [Name("country"), MaxLength(100)] public string? Country { get; init; }

    [Name("location_name"), MaxLength(100)]
    public string? LocationName { get; init; }

    [Name("temperature_celsius")] public required double TemperatureCelsius { get; init; }

    [Name("wind_mph")] public required double WindMph { get; init; }

    [Name("last_updated")] public required DateTime LastUpdated { get; init; }
}
