namespace KISS.Job.Quartz;

/// <summary>
///     Creating extension methods on the IServiceCollection interface.
/// </summary>
public static class QuartzServiceRegistrationExtensions
{
    /// <summary>
    ///     Used to configure QuartzOptions instances.
    /// </summary>
    /// <param name="services">The service container.</param>
    /// <typeparam name="TQuartzOptions">The Quartz configuration.</typeparam>
    public static void ConfigureQuartzOptions<TQuartzOptions>(this IServiceCollection services)
        where TQuartzOptions : QuartzDefaultOptions
    {
        // Configuration from appSettings.json
        services.AddOptions<QuartzOptions>()
            .Configure<IOptions<TQuartzOptions>>((options, config) =>
            {
                options.SchedulerId = config.Value.SchedulerId;
            });
    }

    /// <summary>
    ///     Used to configure Quartz instances.
    /// </summary>
    /// <param name="services">The service container.</param>
    public static void ConfigureQuartzService(this IServiceCollection services)
    {
        // Add the required ILoggerFactory service
        services.AddLogging();

        // Add the required Quartz.NET services
        services.AddQuartz(q =>
        {
            // convert time zones using converter that can handle Windows/Linux differences
            // q.UseTimeZoneConverter();
        });

        // Add the Quartz.NET hosted service
        services.AddQuartzHostedService(
            q => q.WaitForJobsToComplete = true);
    }
}
