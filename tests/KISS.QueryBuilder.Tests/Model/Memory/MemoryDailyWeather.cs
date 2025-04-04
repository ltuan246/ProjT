namespace KISS.QueryBuilder.Tests.Model.Memory;

/// <summary>
///     Represents daily weather data for a specific date and location, summarizing temperature, precipitation, and
///     conditions.
/// </summary>
[SqlTable("DailyWeather")]
public class MemoryDailyWeather
{
    /// <summary>
    ///     Unique identifier for this daily weather record (e.g., a GUID or custom string).
    /// </summary>
    [Key]
    [Name("id")]
    public required Guid Id { get; set; }

    /// <summary>
    ///     Identifier linking this record to a specific location (e.g., foreign key to MemoryLocation table).
    /// </summary>
    [Name("location_id")]
    public Guid? LocationId { get; set; }

    /// <summary>
    ///     The date this weather data pertains to, typically in UTC.
    /// </summary>
    [Name("date")]
    public DateTime? Date { get; set; }

    /// <summary>
    ///     The date represented as a Unix epoch timestamp (seconds since 1970-01-01 UTC).
    /// </summary>
    [Name("date_epoch")]
    public long? DateEpoch { get; set; }

    /// <summary>
    ///     Maximum temperature in Celsius for the day.
    /// </summary>
    [Name("maxtemp_c")]
    public double MaxTempC { get; set; }

    /// <summary>
    ///     Maximum temperature in Fahrenheit for the day.
    /// </summary>
    [Name("maxtemp_f")]
    public double MaxTempF { get; set; }

    /// <summary>
    ///     Minimum temperature in Celsius for the day.
    /// </summary>
    [Name("mintemp_c")]
    public double MinTempC { get; set; }

    /// <summary>
    ///     Minimum temperature in Fahrenheit for the day.
    /// </summary>
    [Name("mintemp_f")]
    public double MinTempF { get; set; }

    /// <summary>
    ///     Average temperature in Celsius for the day.
    /// </summary>
    [Name("avgtemp_c")]
    public double AvgTempC { get; set; }

    /// <summary>
    ///     Average temperature in Fahrenheit for the day.
    /// </summary>
    [Name("avgtemp_f")]
    public double AvgTempF { get; set; }

    /// <summary>
    ///     Maximum wind speed in miles per hour for the day.
    /// </summary>
    [Name("maxwind_mph")]
    public double MaxWindMph { get; set; }

    /// <summary>
    ///     Maximum wind speed in kilometers per hour for the day.
    /// </summary>
    [Name("maxwind_kph")]
    public double MaxWindKph { get; set; }

    /// <summary>
    ///     Total precipitation in millimeters for the day.
    /// </summary>
    [Name("totalprecip_mm")]
    public double TotalPrecipMm { get; set; }

    /// <summary>
    ///     Total precipitation in inches for the day.
    /// </summary>
    [Name("totalprecip_in")]
    public double TotalPrecipIn { get; set; }

    /// <summary>
    ///     Total snowfall in centimeters for the day.
    /// </summary>
    [Name("totalsnow_cm")]
    public double TotalSnowCm { get; set; }

    /// <summary>
    ///     Average visibility in kilometers for the day.
    /// </summary>
    [Name("avgvis_km")]
    public double AvgVisKm { get; set; }

    /// <summary>
    ///     Average visibility in miles for the day.
    /// </summary>
    [Name("avgvis_miles")]
    public double AvgVisMiles { get; set; }

    /// <summary>
    ///     Average relative humidity percentage for the day (0-100).
    /// </summary>
    [Name("avghumidity")]
    public int AvgHumidity { get; set; }

    /// <summary>
    ///     Indicator if rain is expected (0 = no, 1 = yes).
    /// </summary>
    [Name("daily_will_it_rain")]
    public int DailyWillItRain { get; set; }

    /// <summary>
    ///     Percentage chance of rain for the day (0-100).
    /// </summary>
    [Name("daily_chance_of_rain")]
    public int DailyChanceOfRain { get; set; }

    /// <summary>
    ///     Indicator if snow is expected (0 = no, 1 = yes).
    /// </summary>
    [Name("daily_will_it_snow")]
    public int DailyWillItSnow { get; set; }

    /// <summary>
    ///     Percentage chance of snow for the day (0-100).
    /// </summary>
    [Name("daily_chance_of_snow")]
    public int DailyChanceOfSnow { get; set; }

    /// <summary>
    ///     Text description of the weather condition (e.g., "Sunny", "Cloudy").
    /// </summary>
    [Name("condition_text")]
    public string? ConditionText { get; set; }

    /// <summary>
    ///     URL or identifier for an icon representing the weather condition.
    /// </summary>
    [Name("condition_icon")]
    public string? ConditionIcon { get; set; }

    /// <summary>
    ///     Numeric code representing the weather condition (e.g., 1000 for sunny).
    /// </summary>
    [Name("condition_code")]
    public int ConditionCode { get; set; }

    /// <summary>
    ///     UV index value for the day (typically 0-11+).
    /// </summary>
    [Name("uv")]
    public double Uv { get; set; }
}

public class MemoryDailyWeatherModel
{
    /// <summary>
    ///     Unique identifier for this daily weather record (e.g., a GUID or custom string).
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    ///     Identifier linking this record to a specific location (e.g., foreign key to MemoryLocation table).
    /// </summary>
    public Guid? LocationId { get; set; }

    /// <summary>
    ///     The date this weather data pertains to, typically in UTC.
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    ///     The date represented as a Unix epoch timestamp (seconds since 1970-01-01 UTC).
    /// </summary>
    public long? DateEpoch { get; set; }
}
