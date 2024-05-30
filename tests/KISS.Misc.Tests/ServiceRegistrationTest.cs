namespace KISS.Misc.Tests;

public class ServiceRegistrationTest : IDisposable
{
    private ServiceProvider ServiceProvider { get; init; }

    public ServiceRegistrationTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.LifetimeServiceRegistration();
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
        var scopedService = ServiceProvider.GetService<IScopedService>();
        var singletonService = ServiceProvider.GetService<ISingletonService>();
        var transientService = ServiceProvider.GetService<ITransientService>();

        Assert.NotNull(scopedService);
        Assert.NotNull(singletonService);
        Assert.NotNull(transientService);
    }
}