namespace KISS.Misc.Tests;

public class ServiceRegistrationTest
{
    [Fact]
    public void Test1()
    {
        IServiceCollection services = new ServiceCollection();
        services.LifetimeServiceRegistration();
        var serviceProvider = services.BuildServiceProvider();
        var scopedService = serviceProvider.GetService<IScopedService>();
        var singletonService = serviceProvider.GetService<ISingletonService>();
        var transientService = serviceProvider.GetService<ITransientService>();
    }
}