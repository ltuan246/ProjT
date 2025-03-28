namespace KISS.QueryBuilder.Tests.Model;

public class HourlyWeather
{
    [Key]
    public required string Id { get; set; }
    public required string LocationId { get; set; }
    public required DateTime Date { get; set; }
    public long DateEpoch { get; set; }
    public required TimeSpan Time { get; set; }
    public long TimeEpoch { get; set; }
    public double TempC { get; set; }
    public double TempF { get; set; }
    public int IsDay { get; set; }
    public required string ConditionText { get; set; }
    public required string ConditionIcon { get; set; }
    public int ConditionCode { get; set; }
    public double WindMph { get; set; }
    public double WindKph { get; set; }
    public int WindDegree { get; set; }
    public required string WindDir { get; set; }
    public double PressureMb { get; set; }
    public double PressureIn { get; set; }
    public double PrecipMm { get; set; }
    public double PrecipIn { get; set; }
    public double SnowCm { get; set; }
    public int Humidity { get; set; }
    public int Cloud { get; set; }
    public double FeelsLikeC { get; set; }
    public double FeelsLikeF { get; set; }
    public double WindChillC { get; set; }
    public double WindChillF { get; set; }
    public double HeatIndexC { get; set; }
    public double HeatIndexF { get; set; }
    public double DewPointC { get; set; }
    public double DewPointF { get; set; }
    public int WillItRain { get; set; }
    public int ChanceOfRain { get; set; }
    public int WillItSnow { get; set; }
    public int ChanceOfSnow { get; set; }
    public double VisKm { get; set; }
    public double VisMiles { get; set; }
    public double GustMph { get; set; }
    public double GustKph { get; set; }
    public double Uv { get; set; }
}