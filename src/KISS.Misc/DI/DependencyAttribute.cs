namespace KISS.Misc.DI;

/// <summary>
/// Implement dynamic dependencies and lifetime scopes automatically based on attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public abstract class DependencyAttribute : Attribute
{
    /// <summary>
    /// Service Registration.
    /// </summary>
    public abstract Type ServiceType { get; }
}

/// <summary>
/// Implement dynamic dependencies and lifetime scopes automatically based on attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ScopedServiceAttribute(Type serviceType) : DependencyAttribute
{
    /// <summary>
    /// Service Registration.
    /// </summary>
    public override Type ServiceType { get; } = serviceType;
}

/// <summary>
/// Implement dynamic dependencies and lifetime scopes automatically based on attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SingletonServiceAttribute(Type serviceType) : DependencyAttribute
{
    /// <summary>
    /// Service Registration.
    /// </summary>
    public override Type ServiceType { get; } = serviceType;
}

/// <summary>
/// Implement dynamic dependencies and lifetime scopes automatically based on attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class TransientServiceAttribute(Type serviceType) : DependencyAttribute
{
    /// <summary>
    /// Service Registration.
    /// </summary>
    public override Type ServiceType { get; } = serviceType;
}
