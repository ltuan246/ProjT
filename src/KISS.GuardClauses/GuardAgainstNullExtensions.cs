namespace KISS.GuardClauses;

/// <summary>
///     A collection of common guard clauses, implemented as extensions.
/// </summary>
public static class GuardAgainstNullExtensions
{
    /// <summary>
    ///     Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
    /// </summary>
    /// <param name="guard">The Guard Clauses.</param>
    /// <param name="input">The value.</param>
    /// <param name="parameterName">Optional. The name of the parameter that caused the exception.</param>
    /// <param name="message">Optional. A message that describes the error.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns><paramref name="input" /> if the value is not null.</returns>
    /// <exception cref="ArgumentNullException">The exception that is throw if null.</exception>
    public static T Null<T>(
        this IGuardClause guard,
        [NotNull] T? input,
        string? parameterName = null,
        string? message = null)
    {
        _ = guard;
        return Ensure.IsNull(input) switch
        {
            true => throw new ArgumentNullException(parameterName, message),
            false => input
        };
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
    /// </summary>
    /// <param name="guard">The Guard Clauses.</param>
    /// <param name="input">The value.</param>
    /// <param name="parameterName">Optional. The name of the parameter that caused the exception.</param>
    /// <param name="message">Optional. A message that describes the error.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns><paramref name="input" /> if the value is not null.</returns>
    /// <exception cref="ArgumentNullException">The exception that is throw if null.</exception>
    public static T Null<T>(
        this IGuardClause guard,
        [NotNull] T? input,
        string? parameterName = null,
        string? message = null)
        where T : struct
    {
        _ = guard;
        return Ensure.IsNull(input) switch
        {
            true => throw new ArgumentNullException(parameterName, message),
            false => input.Value
        };
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
    /// </summary>
    /// <param name="guard">The Guard Clauses.</param>
    /// <param name="input">The value.</param>
    /// <param name="parameterName">Optional. The name of the parameter that caused the exception.</param>
    /// <param name="message">Optional. A message that describes the error.</param>
    /// <returns><paramref name="input" /> if the value is not null.</returns>
    /// <exception cref="ArgumentNullException">The exception that is throw if null.</exception>
    public static string NullOrEmpty(
        this IGuardClause guard,
        [NotNull] string? input,
        string? parameterName = null,
        string? message = null)
    {
        _ = guard;
        if (Ensure.IsNullOrEmpty(input))
        {
            throw new ArgumentNullException(parameterName, message);
        }

        return input;
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
    /// </summary>
    /// <param name="guard">The Guard Clauses.</param>
    /// <param name="input">The value.</param>
    /// <param name="parameterName">Optional. The name of the parameter that caused the exception.</param>
    /// <param name="message">Optional. A message that describes the error.</param>
    /// <returns><paramref name="input" /> if the value is not null.</returns>
    /// <exception cref="ArgumentNullException">The exception that is throw if null.</exception>
    public static string NullOrEmptyOrWhiteSpace(
        this IGuardClause guard,
        [NotNull] string? input,
        string? parameterName = null,
        string? message = null)
    {
        _ = guard;
        if (Ensure.IsNullOrEmpty(input) || Ensure.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentNullException(parameterName, message);
        }

        return input;
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
    /// </summary>
    /// <param name="guard">The Guard Clauses.</param>
    /// <param name="input">The value.</param>
    /// <param name="parameterName">Optional. The name of the parameter that caused the exception.</param>
    /// <param name="message">Optional. A message that describes the error.</param>
    /// <returns><paramref name="input" /> if the value is not null.</returns>
    /// <exception cref="ArgumentNullException">The exception that is throw if null.</exception>
    public static Guid NullOrEmpty(
        this IGuardClause guard,
        [NotNull] Guid? input,
        string? parameterName = null,
        string? message = null)
    {
        _ = guard;
        return Ensure.IsNullOrEmpty(input) switch
        {
            true => throw new ArgumentNullException(parameterName, message),
            false => input.Value
        };
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
    /// </summary>
    /// <param name="guard">The Guard Clauses.</param>
    /// <param name="input">The value.</param>
    /// <param name="parameterName">Optional. The name of the parameter that caused the exception.</param>
    /// <param name="message">Optional. A message that describes the error.</param>
    /// <typeparam name="T">Must be a value type.</typeparam>
    /// <returns><paramref name="input" /> if the value is not null.</returns>
    /// <exception cref="ArgumentNullException">The exception that is throw if null.</exception>
    public static IEnumerable<T> NullOrEmpty<T>(
        this IGuardClause guard,
        [NotNull] ICollection<T>? input,
        string? parameterName = null,
        string? message = null)
    {
        _ = guard;
        return Ensure.IsNullOrEmpty(input) switch
        {
            true => throw new ArgumentNullException(parameterName, message),
            false => input
        };
    }
}
