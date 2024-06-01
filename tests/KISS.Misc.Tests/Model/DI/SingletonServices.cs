namespace KISS.Misc.Tests.Model.DI;

public interface ISingletonService
{
}

[SingletonService(serviceType: typeof(ISingletonService))]
public class SingletonServiceA : ISingletonService
{
}
