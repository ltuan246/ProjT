namespace KISS.FluentEmail.Senders.Smtp;

/// <summary>
///     Creating extension methods on the IServiceCollection interface.
/// </summary>
public static class SmtpBuilderExtensions
{
    /// <summary>
    ///     Used to configure SmtpClientOptions instances.
    /// </summary>
    /// <param name="services">The service container.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The Section name.</param>
    public static void ConfigureSmtpClientOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "SmtpClientOptions")
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);

        ArgumentNullException.ThrowIfNull(section);

        services
            .AddOptions<SmtpClientOptions>()
            .Bind(section)
            .Validate(options => !string.IsNullOrEmpty(options.Host))
            .Validate(options =>
                !options.UseDefaultCredentials || (options.UseDefaultCredentials
                                                   && !string.IsNullOrEmpty(options.UserName)
                                                   && !string.IsNullOrEmpty(options.Password)))
            .ValidateOnStart();
    }
}
