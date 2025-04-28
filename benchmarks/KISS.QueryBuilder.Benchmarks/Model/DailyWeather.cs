namespace KISS.QueryBuilder.Benchmarks.Model;

/// <summary>
///     Represents daily weather data for a specific date and location, summarizing temperature, precipitation, and
///     conditions.
/// </summary>
[SqlTable("DailyWeather")]
public class DailyWeather
{
    /// <summary>
    ///     Unique identifier for this daily weather record (e.g., a GUID or custom string).
    /// </summary>
    [Key]
    public required string Id { get; set; }

    /// <summary>
    ///     Identifier linking this record to a specific location (e.g., foreign key to Location table).
    /// </summary>
    public required string LocationId { get; set; }

    /// <summary>
    ///     The date this weather data pertains to, typically in UTC.
    /// </summary>
    public required DateTime Date { get; set; }

    /// <summary>
    ///     The date represented as a Unix epoch timestamp (seconds since 1970-01-01 UTC).
    /// </summary>
    public long DateEpoch { get; set; }

    /// <summary>
    ///     Maximum temperature in Celsius for the day.
    /// </summary>
    public double MaxTempC { get; set; }

    /// <summary>
    ///     Maximum temperature in Fahrenheit for the day.
    /// </summary>
    public double MaxTempF { get; set; }

    /// <summary>
    ///     Minimum temperature in Celsius for the day.
    /// </summary>
    public double MinTempC { get; set; }

    /// <summary>
    ///     Minimum temperature in Fahrenheit for the day.
    /// </summary>
    public double MinTempF { get; set; }

    /// <summary>
    ///     Average temperature in Celsius for the day.
    /// </summary>
    public double AvgTempC { get; set; }

    /// <summary>
    ///     Average temperature in Fahrenheit for the day.
    /// </summary>
    public double AvgTempF { get; set; }

    /// <summary>
    ///     Maximum wind speed in miles per hour for the day.
    /// </summary>
    public double MaxWindMph { get; set; }

    /// <summary>
    ///     Maximum wind speed in kilometers per hour for the day.
    /// </summary>
    public double MaxWindKph { get; set; }

    /// <summary>
    ///     Total precipitation in millimeters for the day.
    /// </summary>
    public double TotalPrecipMm { get; set; }

    /// <summary>
    ///     Total precipitation in inches for the day.
    /// </summary>
    public double TotalPrecipIn { get; set; }

    /// <summary>
    ///     Total snowfall in centimeters for the day.
    /// </summary>
    public double TotalSnowCm { get; set; }

    /// <summary>
    ///     Average visibility in kilometers for the day.
    /// </summary>
    public double AvgVisKm { get; set; }

    /// <summary>
    ///     Average visibility in miles for the day.
    /// </summary>
    public double AvgVisMiles { get; set; }

    /// <summary>
    ///     Average relative humidity percentage for the day (0-100).
    /// </summary>
    public int AvgHumidity { get; set; }

    /// <summary>
    ///     Indicator if rain is expected (0 = no, 1 = yes).
    /// </summary>
    public int DailyWillItRain { get; set; }

    /// <summary>
    ///     Percentage chance of rain for the day (0-100).
    /// </summary>
    public int DailyChanceOfRain { get; set; }

    /// <summary>
    ///     Indicator if snow is expected (0 = no, 1 = yes).
    /// </summary>
    public int DailyWillItSnow { get; set; }

    /// <summary>
    ///     Percentage chance of snow for the day (0-100).
    /// </summary>
    public int DailyChanceOfSnow { get; set; }

    /// <summary>
    ///     Text description of the weather condition (e.g., "Sunny", "Cloudy").
    /// </summary>
    public required string ConditionText { get; set; }

    /// <summary>
    ///     URL or identifier for an icon representing the weather condition.
    /// </summary>
    public required string ConditionIcon { get; set; }

    /// <summary>
    ///     Numeric code representing the weather condition (e.g., 1000 for sunny).
    /// </summary>
    public int ConditionCode { get; set; }

    /// <summary>
    ///     UV index value for the day (typically 0-11+).
    /// </summary>
    public double Uv { get; set; }
}
