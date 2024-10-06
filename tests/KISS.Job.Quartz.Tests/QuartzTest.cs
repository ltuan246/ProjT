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
            .Build();

        var schedulerFactory = Services.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();

        TriggerListener triggerListener = new();
        scheduler.ListenerManager.AddTriggerListener(triggerListener);

        await scheduler.Start();
        await scheduler.ScheduleJob(job, trigger);

        await Task.Delay(2000);

        await scheduler.Shutdown();
    }

    public class TriggerListener : ITriggerListener
    {
        public string Name => "QuartzTest";

        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default) => Task.FromResult(true);
    }
}
