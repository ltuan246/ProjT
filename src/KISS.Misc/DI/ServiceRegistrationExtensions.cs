namespace KISS.Misc.DI;

/// <summary>
///     Provides extension methods for dynamic and consistent service registration
///     in the dependency injection container using custom attributes.
/// </summary>
public static class ServiceRegistrationExtensions
{
    /// <summary>
    ///     Registers services in the <see cref="IServiceCollection"/> by scanning all loaded assemblies
    ///     for types decorated with <see cref="ScopedServiceAttribute"/>, <see cref="SingletonServiceAttribute"/>,
    ///     or <see cref="TransientServiceAttribute"/>. Each service is registered with the corresponding lifetime.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    public static void LifetimeServiceRegistration(this IServiceCollection services)
    {
        var targetServices = GetServices();

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

    /// <summary>
    ///     Scans all loaded assemblies for types decorated with custom service lifetime attributes
    ///     and yields their service interface, implementation type, and lifetime.
    /// </summary>
    /// <returns>
    ///     An enumerable of tuples containing the service interface type, implementation type,
    ///     and <see cref="ServiceLifetime"/>.
    /// </returns>
    private static IEnumerable<(Type ServiceInterface, Type ImplementationType, ServiceLifetime ServiceLifetime)>
        GetServices()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                // Check for ScopedServiceAttribute and yield if present
                var scopedService = type.GetCustomAttribute<ScopedServiceAttribute>();
                if (Ensure.IsNotNull(scopedService))
                {
                    yield return (scopedService.ServiceType, type, ServiceLifetime.Scoped);
                }

                // Check for SingletonServiceAttribute and yield if present
                var singletonService = type.GetCustomAttribute<SingletonServiceAttribute>();
                if (Ensure.IsNotNull(singletonService))
                {
                    yield return (singletonService.ServiceType, type, ServiceLifetime.Singleton);
                }

                // Check for TransientServiceAttribute and yield if present
                var transientService = type.GetCustomAttribute<TransientServiceAttribute>();
                if (Ensure.IsNotNull(transientService))
                {
                    yield return (transientService.ServiceType, type, ServiceLifetime.Transient);
                }
            }
        }
    }
}
