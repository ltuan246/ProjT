namespace KISS.QueryBuilder.Tests.Model;

public class Location
{
    [Key]
    public required string Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
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