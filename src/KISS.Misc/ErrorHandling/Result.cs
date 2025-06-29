namespace KISS.Misc.ErrorHandling;

/// <summary>
/// Represents the result of an operation, containing a success flag, value, and error message.
/// </summary>
/// <typeparam name="T">The type of the value returned by the operation.</typeparam>
public sealed record Result<T>(
    bool IsSuccess,
    T? Value,
    string Error)
{
    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <param name="value">The value of the successful result.</param>
    /// <returns>A <see cref="Result{T}"/> representing success.</returns>
    public static Result<T> Success(T value) => new(true, value, string.Empty);

    /// <summary>
    /// Creates a failed result containing the specified error message.
    /// </summary>
    /// <param name="error">The error message for the failed result.</param>
    /// <returns>A <see cref="Result{T}"/> representing failure.</returns>
    public static Result<T> Failure(string error) => new(false, default, error);
}
