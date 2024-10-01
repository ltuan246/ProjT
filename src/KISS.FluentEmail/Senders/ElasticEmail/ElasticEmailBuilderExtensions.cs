namespace KISS.FluentEmail.Senders.ElasticEmail;

/// <summary>
///     Creating extension methods on the IServiceCollection interface.
/// </summary>
public static class ElasticEmailBuilderExtensions
{
    private static int[] AllowedPorts { get; } = [25, 465, 2525];

    /// <summary>
    ///     Used to configure ElasticEmailOptions instances.
    /// </summary>
    /// <param name="services">The service container.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The Section name.</param>
    public static void ConfigureElasticEmailOptions(this IServiceCollection services, IConfiguration configuration, string sectionName = "ElasticEmailOptions")
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);

        ArgumentNullException.ThrowIfNull(section);

        services
            .AddOptions<ElasticEmailOptions>()
            .Bind(section)
            .Validate(options => !string.IsNullOrEmpty(options.Host)
                                 && !string.IsNullOrEmpty(options.UserName)
                                 && !string.IsNullOrEmpty(options.Password)
                                 && AllowedPorts.Contains(options.Port))
            .ValidateOnStart();
    }
}
