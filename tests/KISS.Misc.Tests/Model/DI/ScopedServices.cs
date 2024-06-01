namespace KISS.Misc.Tests.Model.DI;

public interface IScopedService
{
}

[ScopedService(serviceType: typeof(IScopedService))]
public class ScopedServiceA : IScopedService
{
}
