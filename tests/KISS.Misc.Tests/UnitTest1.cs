namespace KISS.Misc.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.ConfigureOptions(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<IOptions<AppOptions>>();
    }
}

[Options("ApplicationOptions")]
public record AppOptions
{
    public required string Title { get; init; }

    public required string ConnectionString { get; init; }

    public int MaximumRetries { get; init; }

    public TimeSpan RetryInterval { get; init; }

    public bool IsLive { get; init; }
}