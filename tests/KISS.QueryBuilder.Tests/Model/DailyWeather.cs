namespace KISS.QueryBuilder.Tests.Model;

public class DailyWeather
{
    [Key]
    public required string Id { get; set; }
    public required string LocationId { get; set; }
    public required DateTime Date { get; set; }
    public long DateEpoch { get; set; }
    public double MaxTempC { get; set; }
    public double MaxTempF { get; set; }
    public double MinTempC { get; set; }
    public double MinTempF { get; set; }
    public double AvgTempC { get; set; }
    public double AvgTempF { get; set; }
    public double MaxWindMph { get; set; }
    public double MaxWindKph { get; set; }
    public double TotalPrecipMm { get; set; }
    public double TotalPrecipIn { get; set; }
    public double TotalSnowCm { get; set; }
    public double AvgVisKm { get; set; }
    public double AvgVisMiles { get; set; }
    public int AvgHumidity { get; set; }
    public int DailyWillItRain { get; set; }
    public int DailyChanceOfRain { get; set; }
    public int DailyWillItSnow { get; set; }
    public int DailyChanceOfSnow { get; set; }
    public required string ConditionText { get; set; }
    public required string ConditionIcon { get; set; }
    public int ConditionCode { get; set; }
    public double Uv { get; set; }
}