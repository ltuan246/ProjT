namespace KISS.FluentEmail.Senders.Mailtrap;

/// <summary>
///     Creating extension methods on the IServiceCollection interface.
/// </summary>
public static class MailtrapBuilderExtensions
{
    private static int[] AllowedPorts { get; } = [25, 465, 2525];

    /// <summary>
    ///     Used to configure MailtrapOptions instances.
    /// </summary>
    /// <param name="services">The service container.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The Section name.</param>
    public static void ConfigureMailtrapOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "MailtrapOptions")
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);

        ArgumentNullException.ThrowIfNull(section);

        services
            .AddOptions<MailtrapOptions>()
            .Bind(section)
            .Validate(options => !string.IsNullOrEmpty(options.Host)
                                 && !string.IsNullOrEmpty(options.UserName)
                                 && !string.IsNullOrEmpty(options.Password)
                                 && AllowedPorts.Contains(options.Port))
            .ValidateOnStart();
    }
}
