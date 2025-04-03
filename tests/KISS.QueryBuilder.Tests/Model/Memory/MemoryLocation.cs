namespace KISS.QueryBuilder.Tests.Model.Memory;

/// <summary>
///     Represents a geographic location with its coordinates and time zone information.
/// </summary>
[SqlTable("Location")]
public class MemoryLocation
{
    /// <summary>
    ///     Unique identifier for this location (e.g., a GUID or custom string).
    /// </summary>
    [Key]
    [Name("id")]
    public required Guid Id { get; set; }

    /// <summary>
    ///     Geographic latitude of the location in degrees (positive = North, negative = South).
    /// </summary>
    [Name("latitude")]
    public double Latitude { get; set; }

    /// <summary>
    ///     Geographic longitude of the location in degrees (positive = East, negative = West).
    /// </summary>
    [Name("longitude")]
    public double Longitude { get; set; }

    /// <summary>
    ///     Time zone identifier for the location (e.g., "Europe/Andorra", "America/New_York").
    /// </summary>
    [Name("tz_id")]
    public required string TzId { get; set; }
}

public class MemoryWeatherModel
{
    public required string Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public required string TzId { get; set; }

    // Navigation properties
    public List<MemoryDailyWeather>? DailyWeathers { get; set; }
}
