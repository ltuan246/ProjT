namespace KISS.Misc;

public static class IConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddOptions(this IConfigurationBuilder configuration, string appSettings = "appsettings.json")
    {
        configuration.AddJsonFile(appSettings, optional: true, reloadOnChange: true);

        return configuration;
    }
}