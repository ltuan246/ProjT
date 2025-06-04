namespace KISS.Misc.Tests;

public class ConfigureOptionsTest : IDisposable
{
    private ServiceProvider Services { get; init; }

    public ConfigureOptionsTest()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();

        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.ConfigureOptions(configuration);
        Services = serviceCollection.BuildServiceProvider();
    }

    public void Dispose()
    {
        Services.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// [MethodUnderTest]_[Scenario]_[ExpectedResult]
    /// </summary>
    [Fact]
    public void GetService_BuildServiceProvider_ReturnsService()
    {
        IOptions<AppSettings>? options = Services.GetService<IOptions<AppSettings>>();
        Assert.NotNull(options);
    }
}
