namespace KISS.FluentEmail.Models;

/// <summary>
///     Initializes a new instance of the <see cref="SendingMessage" /> class.
/// </summary>
/// <param name="FromAddress">The sender’s email address.</param>
/// <param name="MailSubject">The subject of the message.</param>
/// <param name="MailBody">The body of the message.</param>
/// <param name="IsHtml">Checks whether the body of the message is text/html.</param>
public record SendingMessage(MailingAddress FromAddress, string MailSubject, string MailBody, bool IsHtml = false)
{
    /// <summary>
    ///     An array of email addresses for the primary recipients of the message.
    /// </summary>
    public IEnumerable<MailingAddress> ToAddresses { get; } = [];

    /// <summary>
    ///     An array of email addresses for the secondary recipients of the message.
    /// </summary>
    public IEnumerable<MailingAddress> CcAddresses { get; } = [];

    /// <summary>
    ///     An array of email addresses for the concealed tertiary recipients of the message.
    /// </summary>
    public IEnumerable<MailingAddress> BccAddresses { get; } = [];

    /// <summary>
    ///     An array of email addresses to use when replying to the message.
    /// </summary>
    public IEnumerable<MailingAddress> ReplyToAddresses { get; } = [];

    /// <summary>
    ///     An array of attach files to the message.
    /// </summary>
    public IEnumerable<FileAttachment> Attachments { get; } = [];
}
