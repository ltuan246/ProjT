namespace KISS.FluentEmail.Models;

/// <summary>
/// SendResponse.
/// </summary>
public class SendResponse
{
    /// <summary>
    ///     An array of email addresses for the primary recipients of the message.
    /// </summary>
    public ICollection<string> ErrorMessages { get; } = [];

    /// <summary>
    /// Successful.
    /// </summary>
    public bool IsSuccessful => ErrorMessages.Count == 0;
}
