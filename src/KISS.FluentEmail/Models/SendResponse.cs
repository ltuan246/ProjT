namespace KISS.FluentEmail.Models;

/// <summary>
///     Send messages response.
/// </summary>
public class SendResponse
{
    /// <summary>
    ///     An array of email addresses for the primary recipients of the message.
    /// </summary>
    public ICollection<string> ErrorMessages { get; } = [];

    /// <summary>
    ///     A successful send returns an empty ErrorMessages.
    /// </summary>
    public bool IsSuccessful => ErrorMessages.Count == 0;
}
