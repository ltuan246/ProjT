namespace KISS.FluentEmail.Senders.MailKit;

/// <summary>
///     Creating extension methods on the IServiceCollection interface.
/// </summary>
public static class MailKitBuilderExtensions
{
    /// <summary>
    ///     Used to configure MailKitOptions instances.
    /// </summary>
    /// <param name="services">The service container.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The Section name.</param>
    public static void ConfigureMailKitOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "MailKitOptions")
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);

        ArgumentNullException.ThrowIfNull(section);

        services
            .AddOptions<MailKitOptions>()
            .Bind(section)
            .Validate(options =>
            {
                if (string.IsNullOrEmpty(options.Host))
                {
                    return false;
                }

                if (!options.UseDefaultCredentials)
                {
                    if (string.IsNullOrEmpty(options.UserName) || string.IsNullOrEmpty(options.Password))
                    {
                        return false;
                    }
                }

                return true;
            })
            .ValidateOnStart();
    }
}
