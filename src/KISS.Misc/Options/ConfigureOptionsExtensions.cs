namespace KISS.Misc.Options;

/// <summary>
///     Dynamically invoking a generic method.
/// </summary>
public static class ConfigureOptionsExtensions
{
    private const string ConfigureOptionsMethodName = nameof(OptionsConfigurationServiceCollectionExtensions.Configure);

    /// <summary>
    ///     Define a generic method with reflection emit.
    ///     https://learn.microsoft.com/en-us/dotnet/fundamentals/reflection/how-to-define-a-generic-method-with-reflection-emit.
    /// </summary>
    /// <param name="services">The service container.</param>
    /// <param name="configuration">The configuration.</param>
    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        Guard.Against.Null(configuration);

        var configOpts =
            typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod(ConfigureOptionsMethodName, [typeof(IServiceCollection), typeof(IConfiguration)]);

        Guard.Against.Null(configOpts);

        var targetServices = GetOptions();

        foreach ((var service, var sectionName) in targetServices)
        {
            var section = configuration.GetSection(sectionName);
            configOpts.MakeGenericMethod(service).Invoke(null, [services, section]);
        }
    }

    private static IEnumerable<(Type Service, string SectionName)> GetOptions()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var options = type.GetCustomAttribute<OptionsAttribute>();
                if (Ensure.IsNotNull(options))
                {
                    yield return (type, options.SectionName);
                }
            }
        }
    }
}
