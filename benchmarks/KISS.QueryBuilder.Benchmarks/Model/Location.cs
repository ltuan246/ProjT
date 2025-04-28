namespace KISS.QueryBuilder.Benchmarks.Model;

/// <summary>
///     Represents a geographic location with its coordinates and time zone information.
/// </summary>
[SqlTable("Location")]
public class Location
{
    /// <summary>
    ///     Unique identifier for this location (e.g., a GUID or custom string).
    /// </summary>
    [Key]
    public required string Id { get; set; }

    /// <summary>
    ///     Geographic latitude of the location in degrees (positive = North, negative = South).
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    ///     Geographic longitude of the location in degrees (positive = East, negative = West).
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    ///     Time zone identifier for the location (e.g., "Europe/Andorra", "America/New_York").
    /// </summary>
    public required string TzId { get; set; }
}

public class WeatherModel
{
    public required string Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public required string TzId { get; set; }

    // Navigation properties
    public List<DailyWeather>? DailyWeathers { get; set; }
    public Astronomy? Astro { get; set; }
    public List<HourlyWeather>? HourlyWeathers { get; set; }
}
