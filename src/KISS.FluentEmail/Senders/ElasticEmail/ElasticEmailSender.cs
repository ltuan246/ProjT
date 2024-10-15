namespace KISS.FluentEmail.Senders.ElasticEmail;

/// <summary>
///     Send email via ElasticEmail.
/// </summary>
public class ElasticEmailSender(IOptions<ElasticEmailOptions> options)
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
