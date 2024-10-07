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
    public async void SchedulingBackgroundTasks_ScheduleJob_TriggerFired()
    {
        // Arrange
        const string jobName = "job";
        const string jobGroup = "group";
        const string triggerName = "trigger";
        const string triggerGroup = "group";

        IJobDetail job = JobBuilder.Create<DummyJob>()
            .WithIdentity(jobName, jobGroup)
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity(triggerName, triggerGroup)
            .Build();

        var schedulerFactory = Services.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();

        TriggerListener triggerListener = new();
        scheduler.ListenerManager.AddTriggerListener(triggerListener);

        SchedulerListener schedulerListener = new();
        scheduler.ListenerManager.AddSchedulerListener(schedulerListener);

        // Act
        await scheduler.Start();
        await scheduler.ScheduleJob(job, trigger);

        await Task.Delay(2000);

        await scheduler.Shutdown();

        // Assert
        Assert.Equal(triggerListener.FireCount, 1);
    }
}
