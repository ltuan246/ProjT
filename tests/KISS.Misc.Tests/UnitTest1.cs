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
    public string Title { get; set; } = "";

    public string ConnectionString { get; set; } = "";

    public int MaximumRetries { get; set; }

    public TimeSpan RetryInterval { get; set; }

    public bool IsLive { get; set; }
}