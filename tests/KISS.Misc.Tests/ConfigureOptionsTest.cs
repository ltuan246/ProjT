namespace KISS.Misc.Tests;

public class ConfigureOptionsTest : IDisposable
{
    private ServiceProvider ServiceProvider { get; init; }

    public ConfigureOptionsTest()
    {
        var configuration = new ConfigurationBuilder()
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

    [Fact]
    public void Test1()
    {
        var options = ServiceProvider.GetService<IOptions<AppSettings>>();
        Assert.NotNull(options);
    }
}