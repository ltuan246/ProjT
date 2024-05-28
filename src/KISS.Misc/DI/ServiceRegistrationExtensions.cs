using KISS.Misc.GuardClauses;

namespace KISS.Misc.DI;

public static class ServiceRegistrationExtensions
{
    public static void LifetimeServiceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        IEnumerable<(Type serviceInterface, Type implementationType, ServiceLifetime serviceLifetime)> targetServices =
            GetServices();

        foreach ((Type serviceInterface, Type implementationType, ServiceLifetime serviceLifetime) in targetServices)
        {
            _ = serviceLifetime switch
            {
                ServiceLifetime.Scoped => services.AddScoped(serviceInterface, implementationType),
                ServiceLifetime.Singleton => services.AddSingleton(serviceInterface, implementationType),
                ServiceLifetime.Transient => services.AddTransient(serviceInterface, implementationType),
                _ => services
            };
        }
    }

    private static IEnumerable<(Type serviceInterface, Type implementationType, ServiceLifetime serviceLifetime)>
        GetServices()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                Type[] implementedInterfaces = type.GetInterfaces();

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