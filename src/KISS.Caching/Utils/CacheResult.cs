namespace KISS.Caching.Utils;

/// <summary>
///     Represents the result of a cache lookup, indicating if a value was found and providing access to it.
/// </summary>
/// <typeparam name="T">The reference type of the cached value.</typeparam>
public sealed record CacheResult<T>(T? Value = default, bool HasValue = false)
{
    /// <summary>
    ///     Gets the cached value if present; otherwise, throws an <see cref="InvalidOperationException" />.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if no value is present in the cache result.</exception>
    private T GetValue => HasValue ? Value! : throw new InvalidOperationException("CacheResult has no value.");

    /// <summary>
    ///     Implicitly converts a <see cref="CacheResult{T}" /> to its value. Returns <c>default</c> if the result is
    ///     <c>null</c>.
    /// </summary>
    /// <param name="result">The cache result to convert.</param>
    public static implicit operator T(CacheResult<T> result) => result is null ? default! : result.GetValue;

    /// <summary>
    ///     Implicitly wraps a value in a <see cref="CacheResult{T}" />. If the value is <c>null</c>, returns a null result.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    public static implicit operator CacheResult<T>(T value) => value == null ? Null() : Success(value);

    /// <summary>
    ///     Creates a successful cache result containing the specified value.
    /// </summary>
    /// <param name="value">The value to store in the cache result.</param>
    /// <returns>A <see cref="CacheResult{T}" /> representing a successful cache lookup.</returns>
    public static CacheResult<T> Success(T value) => new() { Value = value, HasValue = true };

    /// <summary>
    ///     Creates a cache result representing a missing or <c>null</c> value.
    /// </summary>
    /// <returns>A <see cref="CacheResult{T}" /> with no value.</returns>
    public static CacheResult<T> Null() => new() { Value = default, HasValue = false };
}
