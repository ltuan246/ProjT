namespace KISS.Caching.Mechanisms;

/// <summary>
///     Configuration options for cache mechanisms in KISS.Caching.
///     <para>
///         Use this record to specify how long items should stay in the cache, either by setting a sliding expiration
///         (which resets every time the item is accessed)
///         or an absolute expiration (which removes the item after a fixed period, regardless of access).
///     </para>
/// </summary>
/// <param name="SlidingExpiration">
///     Optional. The time interval to keep an item alive in the cache since its last access.
///     If null, sliding expiration is not used.
/// </param>
/// <param name="AbsoluteExpiration">
///     Optional. The maximum time an item can stay in the cache, regardless of access. If
///     null, absolute expiration is not used.
/// </param>
public sealed record CacheMechanismOptions(TimeSpan? SlidingExpiration, TimeSpan? AbsoluteExpiration);
