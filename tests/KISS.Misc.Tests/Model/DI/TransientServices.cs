namespace KISS.Misc.Tests.Model.DI;

public interface ITransientService
{
}

[TransientService(serviceType: typeof(ITransientService))]
public class TransientServiceA : ITransientService
{
}
