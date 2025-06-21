namespace KISS.Caching.Utils;

/// <summary>
///     Attribute for marking classes with a specific cache strategy, enabling dynamic dependency resolution and strategy
///     selection.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CacheStrategyAttribute(CacheStrategy strategy) : Attribute
{
    /// <summary>
    ///     Gets the cache strategy associated with the attributed class.
    /// </summary>
    public CacheStrategy Strategy { get; } = strategy;
}
