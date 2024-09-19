namespace KISS.FluentEmail.Senders.Smtp;

/// <summary>
/// FluentEmailSmtpBuilderExtensions.
/// </summary>
public static class FluentEmailSmtpBuilderExtensions
{
    /// <summary>
    /// ConfigureSmtpClientOptions.
    /// </summary>
    /// <param name="services">The service container.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The Section name.</param>
    public static void ConfigureSmtpClientOptions(this IServiceCollection services, IConfiguration configuration, string sectionName = "SmtpClientOptions")
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);

        ArgumentNullException.ThrowIfNull(section);

        services
            .AddOptions<SmtpClientOptions>()
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
