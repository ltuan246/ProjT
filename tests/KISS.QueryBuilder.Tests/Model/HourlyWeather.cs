namespace KISS.QueryBuilder.Tests.Model;

/// <summary>
///     Represents hourly weather data for a specific time and location, detailing temperature, wind, and precipitation.
/// </summary>
[SqlTable("HourlyWeather")]
public class HourlyWeather
{
    /// <summary>
    ///     Unique identifier for this hourly weather record (e.g., a GUID or custom string).
    /// </summary>
    [Key, KeyBuilder]
    public required string Id { get; set; }

    /// <summary>
    ///     Identifier linking this record to a specific location (e.g., foreign key to Location table).
    /// </summary>
    public required string LocationId { get; set; }

    /// <summary>
    ///     The date this hourly data pertains to, typically in UTC.
    /// </summary>
    public required DateTime Date { get; set; }

    /// <summary>
    ///     The date represented as a Unix epoch timestamp (seconds since 1970-01-01 UTC).
    /// </summary>
    public long DateEpoch { get; set; }

    /// <summary>
    ///     The specific time of this hourly record within the day, in local time (e.g., 14:00:00).
    /// </summary>
    public required TimeSpan Time { get; set; }

    /// <summary>
    ///     The time represented as a Unix epoch timestamp (seconds since 1970-01-01 UTC).
    /// </summary>
    public long TimeEpoch { get; set; }

    /// <summary>
    ///     Temperature in Celsius at this hour.
    /// </summary>
    public double TempC { get; set; }

    /// <summary>
    ///     Temperature in Fahrenheit at this hour.
    /// </summary>
    public double TempF { get; set; }

    /// <summary>
    ///     Indicator if this hour is during daylight (0 = night, 1 = day).
    /// </summary>
    public bool IsDay { get; set; }

    /// <summary>
    ///     Text description of the weather condition at this hour (e.g., "Rainy", "Clear").
    /// </summary>
    public required string ConditionText { get; set; }

    /// <summary>
    ///     URL or identifier for an icon representing the weather condition at this hour.
    /// </summary>
    public required string ConditionIcon { get; set; }

    /// <summary>
    ///     Numeric code representing the weather condition at this hour (e.g., 1003 for partly cloudy).
    /// </summary>
    public int ConditionCode { get; set; }

    /// <summary>
    ///     Wind speed in miles per hour at this hour.
    /// </summary>
    public double WindMph { get; set; }

    /// <summary>
    ///     Wind speed in kilometers per hour at this hour.
    /// </summary>
    public double WindKph { get; set; }

    /// <summary>
    ///     Wind direction in degrees (0-359, where 0 is North).
    /// </summary>
    public int WindDegree { get; set; }

    /// <summary>
    ///     Wind direction as a cardinal direction (e.g., "N", "SW").
    /// </summary>
    public required string WindDir { get; set; }

    /// <summary>
    ///     Atmospheric pressure in millibars at this hour.
    /// </summary>
    public double PressureMb { get; set; }

    /// <summary>
    ///     Atmospheric pressure in inches of mercury at this hour.
    /// </summary>
    public double PressureIn { get; set; }

    /// <summary>
    ///     Precipitation amount in millimeters for this hour.
    /// </summary>
    public double PrecipMm { get; set; }

    /// <summary>
    ///     Precipitation amount in inches for this hour.
    /// </summary>
    public double PrecipIn { get; set; }

    /// <summary>
    ///     Snowfall amount in centimeters for this hour.
    /// </summary>
    public double SnowCm { get; set; }

    /// <summary>
    ///     Relative humidity percentage at this hour (0-100).
    /// </summary>
    public int Humidity { get; set; }

    /// <summary>
    ///     Cloud cover percentage at this hour (0-100).
    /// </summary>
    public int Cloud { get; set; }

    /// <summary>
    ///     "Feels like" temperature in Celsius, accounting for wind chill or heat index.
    /// </summary>
    public double FeelsLikeC { get; set; }

    /// <summary>
    ///     "Feels like" temperature in Fahrenheit.
    /// </summary>
    public double FeelsLikeF { get; set; }

    /// <summary>
    ///     Wind chill temperature in Celsius at this hour.
    /// </summary>
    public double WindChillC { get; set; }

    /// <summary>
    ///     Wind chill temperature in Fahrenheit at this hour.
    /// </summary>
    public double WindChillF { get; set; }

    /// <summary>
    ///     Heat index temperature in Celsius at this hour.
    /// </summary>
    public double HeatIndexC { get; set; }

    /// <summary>
    ///     Heat index temperature in Fahrenheit at this hour.
    /// </summary>
    public double HeatIndexF { get; set; }

    /// <summary>
    ///     Dew point temperature in Celsius at this hour.
    /// </summary>
    public double DewPointC { get; set; }

    /// <summary>
    ///     Dew point temperature in Fahrenheit at this hour.
    /// </summary>
    public double DewPointF { get; set; }

    /// <summary>
    ///     Indicator if rain is expected this hour (0 = no, 1 = yes).
    /// </summary>
    public int WillItRain { get; set; }

    /// <summary>
    ///     Percentage chance of rain this hour (0-100).
    /// </summary>
    public int ChanceOfRain { get; set; }

    /// <summary>
    ///     Indicator if snow is expected this hour (0 = no, 1 = yes).
    /// </summary>
    public int WillItSnow { get; set; }

    /// <summary>
    ///     Percentage chance of snow this hour (0-100).
    /// </summary>
    public int ChanceOfSnow { get; set; }

    /// <summary>
    ///     Visibility in kilometers at this hour.
    /// </summary>
    public double VisKm { get; set; }

    /// <summary>
    ///     Visibility in miles at this hour.
    /// </summary>
    public double VisMiles { get; set; }

    /// <summary>
    ///     Wind gust speed in miles per hour at this hour.
    /// </summary>
    public double GustMph { get; set; }

    /// <summary>
    ///     Wind gust speed in kilometers per hour at this hour.
    /// </summary>
    public double GustKph { get; set; }

    /// <summary>
    ///     UV index value at this hour (typically 0-11+).
    /// </summary>
    public double Uv { get; set; }
}
