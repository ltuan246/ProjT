namespace KISS.Misc.ErrorHandling;

/// <summary>
/// Represents an error with a message and HTTP status code.
/// </summary>
public sealed record HttpError(string Message, int StatusCode)
{
    /// <summary>
    /// Creates a 404 Not Found error.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>An <see cref="HttpError"/> with status code 404.</returns>
    public static HttpError NotFound(string message) => new(message, 404);

    /// <summary>
    /// Creates a 400 Bad Request error.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>An <see cref="HttpError"/> with status code 400.</returns>
    public static HttpError BadRequest(string message) => new(message, 400);

    /// <summary>
    /// Creates a 401 Unauthorized error.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>An <see cref="HttpError"/> with status code 401.</returns>
    public static HttpError Unauthorized(string message) => new(message, 401);

    /// <summary>
    /// Creates a 403 Forbidden error.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>An <see cref="HttpError"/> with status code 403.</returns>
    public static HttpError Forbidden(string message) => new(message, 403);

    /// <summary>
    /// Creates a 500 Internal Server Error.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>An <see cref="HttpError"/> with status code 500.</returns>
    public static HttpError InternalServerError(string message) => new(message, 500);
}
