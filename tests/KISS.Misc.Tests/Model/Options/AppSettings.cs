namespace KISS.Misc.Tests.Model.Options;

[Options("ApplicationOptions")]
public class AppSettings
{
    public required string Title { get; init; }

    public required string ConnectionString { get; init; }

    public int MaximumRetries { get; init; }

    public TimeSpan RetryInterval { get; init; }

    public bool IsLive { get; init; }
}
