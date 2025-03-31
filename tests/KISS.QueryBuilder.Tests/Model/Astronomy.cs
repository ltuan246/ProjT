namespace KISS.QueryBuilder.Tests.Model;

/// <summary>
///     Represents astronomical data for a specific date and location, including sun and moon events.
/// </summary>
[SqlTable("Astronomy")]
public class Astronomy
{
    /// <summary>
    ///     Unique identifier for this astronomy record (e.g., a GUID or custom string).
    /// </summary>
    [Key]
    public required string Id { get; set; }

    /// <summary>
    ///     Identifier linking this record to a specific location (e.g., foreign key to Location table).
    /// </summary>
    public required string LocationId { get; set; }

    /// <summary>
    ///     The date this astronomical data pertains to, typically in UTC.
    /// </summary>
    public required DateTime Date { get; set; }

    /// <summary>
    ///     The date represented as a Unix epoch timestamp (seconds since 1970-01-01 UTC).
    /// </summary>
    public long DateEpoch { get; set; }

    /// <summary>
    ///     Time of sunrise on the given date, in local time (e.g., "06:45 AM").
    /// </summary>
    public required string Sunrise { get; set; }

    /// <summary>
    ///     Time of sunset on the given date, in local time (e.g., "07:30 PM").
    /// </summary>
    public required string Sunset { get; set; }

    /// <summary>
    ///     Time of moonrise on the given date, in local time (e.g., "09:15 PM").
    /// </summary>
    public required string Moonrise { get; set; }

    /// <summary>
    ///     Time of moonset on the given date, in local time (e.g., "08:00 AM").
    /// </summary>
    public required string Moonset { get; set; }

    /// <summary>
    ///     Description of the moon's phase (e.g., "Full Moon", "Waxing Crescent").
    /// </summary>
    public required string MoonPhase { get; set; }

    /// <summary>
    ///     Percentage of the moon illuminated, from 0 (new moon) to 100 (full moon).
    /// </summary>
    public int MoonIllumination { get; set; }
}
