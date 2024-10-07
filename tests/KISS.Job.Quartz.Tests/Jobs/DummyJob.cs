namespace KISS.Job.Quartz.Tests.Jobs;

public class DummyJob : IJob
{
    public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
}
