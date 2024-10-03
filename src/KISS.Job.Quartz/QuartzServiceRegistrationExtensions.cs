namespace KISS.Job.Quartz;

/// <summary>
///     Creating extension methods on the IServiceCollection interface.
/// </summary>
public static class QuartzServiceRegistrationExtensions
{
    /// <summary>
    ///     Used to configure Quartz instances.
    /// </summary>
    /// <param name="services">The service container.</param>
    public static void ConfigureQuartz(this IServiceCollection services)
    {
        // Add the required Quartz.NET services
        services.AddQuartz(q =>
        {
        });

        // Add the Quartz.NET hosted service
        services.AddQuartzHostedService(
            q => q.WaitForJobsToComplete = true);
    }
}
