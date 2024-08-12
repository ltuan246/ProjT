namespace KISS.Misc.Options;

/// <summary>
/// Dynamically invoking a generic method.
/// </summary>
public static class ConfigureOptionsExtensions
{
    private const string ConfigureOptionsMethodName = nameof(OptionsConfigurationServiceCollectionExtensions.Configure);

    /// <summary>
    /// Define a generic method with reflection emit.
    /// https://learn.microsoft.com/en-us/dotnet/fundamentals/reflection/how-to-define-a-generic-method-with-reflection-emit .
    /// </summary>
    /// <param name="services">The service container.</param>
    /// <param name="configuration">The configuration.</param>
    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        Guard.Against.Null(configuration);

        MethodInfo? configOpts =
            typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod(ConfigureOptionsMethodName, [typeof(IServiceCollection), typeof(IConfiguration)]);

        Guard.Against.Null(configOpts);

        IEnumerable<(Type Service, string SectionName)> targetServices = GetOptions();

        foreach ((Type service, string sectionName) in targetServices)
        {
            IConfigurationSection section = configuration.GetSection(sectionName);
            configOpts.MakeGenericMethod(service).Invoke(null, [services, section]);
        }
    }

    private static IEnumerable<(Type Service, string SectionName)> GetOptions()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                OptionsAttribute? options = type.GetCustomAttribute<OptionsAttribute>();
                if (Ensure.IsNotNull(options))
                {
                    yield return (type, options.SectionName);
                }
            }
        }
    }
}
