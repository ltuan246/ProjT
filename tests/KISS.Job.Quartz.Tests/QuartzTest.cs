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
        IJobDetail job = JobBuilder.Create<DummyJob>()
            .WithIdentity("job1", "group1")
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x.WithIntervalInSeconds(1))
            .Build();

        var schedulerFactory = Services.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();

        await scheduler.ScheduleJob(job, trigger);
        await scheduler.Start();

        await Task.Delay(2000);

        await scheduler.ResumeAll();
        await scheduler.Shutdown();
    }
}
