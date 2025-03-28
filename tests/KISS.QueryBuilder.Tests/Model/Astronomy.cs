namespace KISS.QueryBuilder.Tests.Model;

public class Astronomy
{
    [Key]
    public required string Id { get; set; }
    public required string LocationId { get; set; }
    public required DateTime Date { get; set; }
    public long DateEpoch { get; set; }
    public required string Sunrise { get; set; }
    public required string Sunset { get; set; }
    public required string Moonrise { get; set; }
    public required string Moonset { get; set; }
    public required string MoonPhase { get; set; }
    public int MoonIllumination { get; set; }
}