namespace KISS.Misc.Tests;

public class ConfigureOptionsTest : IDisposable
{
    private ServiceProvider ServiceProvider { get; init; }

    public ConfigureOptionsTest()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();

        IServiceCollection services = new ServiceCollection();
        services.ConfigureOptions(configuration);
        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        ServiceProvider.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// [MethodUnderTest]_[Scenario]_[ExpectedResult]
    /// </summary>
    [Fact]
    public void GetService_BuildServiceProvider_ReturnsService()
    {
        IOptions<AppSettings>? options = ServiceProvider.GetService<IOptions<AppSettings>>();
        Assert.NotNull(options);
    }
}
