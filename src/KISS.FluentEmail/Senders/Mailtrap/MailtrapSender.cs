namespace KISS.FluentEmail.Senders.Mailtrap;

/// <summary>
///     Send email via Mailtrap.
/// </summary>
public class MailtrapSender(IOptions<MailtrapOptions> options)
{
    /// <summary>
    ///     Send the specified message.
    /// </summary>
    /// <param name="sendingMessage">Specified message.</param>
    /// <returns>Send Response.</returns>
    public SendResponse Send([NotNull] SendingMessage sendingMessage)
    {
        SmtpSender sender = new(options.Value);
        return sender.Send(sendingMessage);
    }
}
