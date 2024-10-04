namespace KISS.Job.Quartz.Tests;

public class QuartzTest : IDisposable
{
    private ServiceProvider Services { get; init; }

    public QuartzTest()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.ConfigureQuartzOptions<QuartzCustomOptions>();
        serviceCollection.ConfigureQuartzService();
        Services = serviceCollection.BuildServiceProvider();
    }

    public void Dispose()
    {
        Services.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async void Test1()
    {
        var schedulerFactory = Services.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();

        await scheduler.Start();
        await scheduler.Shutdown();
    }
}
