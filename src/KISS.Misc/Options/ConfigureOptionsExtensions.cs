using KISS.Misc.GuardClauses;

namespace KISS.Misc.Options;

public static class ConfigureOptionsExtensions
{
    private const string ConfigureOptionsMethodName = nameof(OptionsConfigurationServiceCollectionExtensions.Configure);

    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/fundamentals/reflection/how-to-define-a-generic-method-with-reflection-emit
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        MethodInfo? configOpts =
            typeof(OptionsConfigurationServiceCollectionExtensions).GetMethod(ConfigureOptionsMethodName,
                [typeof(IServiceCollection), typeof(IConfiguration)]);

        Guard.Against.Null(configOpts);

        IEnumerable<(Type service, string sectionName)> targetServices = GetOptions();

        foreach ((Type service, string sectionName) in targetServices)
        {
            IConfigurationSection section = configuration.GetSection(sectionName);
            configOpts.MakeGenericMethod(service).Invoke(null, [services, section]);
        }
    }

    private static IEnumerable<(Type service, string sectionName)> GetOptions()
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