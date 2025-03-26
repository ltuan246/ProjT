namespace KISS.QueryBuilder.Tests.Model;

public class Country
{
    [Name("CountryCode")]
    public required string CountryCode { get; set; }

    [Name("Latitude")]
    public required string Latitude { get; set; }

    [Name("Longitude")]
    public required string Longitude { get; set; }
}