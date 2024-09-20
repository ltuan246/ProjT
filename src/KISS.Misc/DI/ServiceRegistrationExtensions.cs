namespace KISS.Misc.DI;

/// <summary>
///     Simplifying Dynamic and Consistent Service Registration.
/// </summary>
public static class ServiceRegistrationExtensions
{
    /// <summary>
    ///     Dynamic Registration with Custom Attributes.
    /// </summary>
    /// <param name="services">The service container.</param>
    public static void LifetimeServiceRegistration(this IServiceCollection services)
    {
        var targetServices =
            GetServices();

        foreach ((var serviceInterface, var implementationType, var serviceLifetime) in targetServices)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(serviceInterface, implementationType);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(serviceInterface, implementationType);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(serviceInterface, implementationType);
                    break;
            }
        }
    }

    private static IEnumerable<(Type ServiceInterface, Type ImplementationType, ServiceLifetime ServiceLifetime)>
        GetServices()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                // Type[] implementedInterfaces = type.GetInterfaces();

                var scopedService = type.GetCustomAttribute<ScopedServiceAttribute>();
                if (Ensure.IsNotNull(scopedService))
                {
                    yield return (scopedService.ServiceType, type, ServiceLifetime.Scoped);
                }

                var singletonService = type.GetCustomAttribute<SingletonServiceAttribute>();
                if (Ensure.IsNotNull(singletonService))
                {
                    yield return (singletonService.ServiceType, type, ServiceLifetime.Singleton);
                }

                var transientService = type.GetCustomAttribute<TransientServiceAttribute>();
                if (Ensure.IsNotNull(transientService))
                {
                    yield return (transientService.ServiceType, type, ServiceLifetime.Transient);
                }
            }
        }
    }
}
