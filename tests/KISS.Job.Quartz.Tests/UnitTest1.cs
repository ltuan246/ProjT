namespace KISS.Job.Quartz.Tests;

public class UnitTest1 : IDisposable
{
    private ServiceProvider Services { get; init; }

    public UnitTest1()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.ConfigureQuartz();
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
