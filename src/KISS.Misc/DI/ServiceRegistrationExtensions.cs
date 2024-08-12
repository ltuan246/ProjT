namespace KISS.Misc.DI;

/// <summary>
/// Simplifying Dynamic and Consistent Service Registration.
/// </summary>
public static class ServiceRegistrationExtensions
{
    /// <summary>
    /// Dynamic Registration with Custom Attributes.
    /// </summary>
    /// <param name="services">The service container.</param>
    public static void LifetimeServiceRegistration(this IServiceCollection services)
    {
        IEnumerable<(Type ServiceInterface, Type ImplementationType, ServiceLifetime ServiceLifetime)> targetServices =
            GetServices();

        foreach ((Type serviceInterface, Type implementationType, ServiceLifetime serviceLifetime) in targetServices)
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
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                // Type[] implementedInterfaces = type.GetInterfaces();

                ScopedServiceAttribute? scopedService = type.GetCustomAttribute<ScopedServiceAttribute>();
                if (Ensure.IsNotNull(scopedService))
                {
                    yield return (scopedService.ServiceType, type, ServiceLifetime.Scoped);
                }

                SingletonServiceAttribute? singletonService = type.GetCustomAttribute<SingletonServiceAttribute>();
                if (Ensure.IsNotNull(singletonService))
                {
                    yield return (singletonService.ServiceType, type, ServiceLifetime.Singleton);
                }

                TransientServiceAttribute? transientService = type.GetCustomAttribute<TransientServiceAttribute>();
                if (Ensure.IsNotNull(transientService))
                {
                    yield return (transientService.ServiceType, type, ServiceLifetime.Transient);
                }
            }
        }
    }
}
