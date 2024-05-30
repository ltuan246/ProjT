namespace KISS.Misc.DI;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public abstract class DependencyAttribute : Attribute
{
    public abstract Type ServiceType { get; }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ScopedServiceAttribute(Type serviceType) : DependencyAttribute
{
    public override Type ServiceType { get; } = serviceType;
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SingletonServiceAttribute(Type serviceType) : DependencyAttribute
{
    public override Type ServiceType { get; } = serviceType;
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class TransientServiceAttribute(Type serviceType) : DependencyAttribute
{
    public override Type ServiceType { get; } = serviceType;
}