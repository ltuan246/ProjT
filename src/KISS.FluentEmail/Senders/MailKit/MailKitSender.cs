using MailKit.Net.Smtp;
using MimeKit;

namespace KISS.FluentEmail.Senders.MailKit;

/// <summary>
///     Send emails using the MailKit email library.
/// </summary>
public class MailKitSender(IOptions<MailKitOptions> options)
{
    /// <summary>
    ///     Send the specified message.
    /// </summary>
    /// <param name="sendingMessage">Specified message.</param>
    public void Send([NotNull] SendingMessage sendingMessage)
    {
        using MimeMessage mailMessage = CreateMailMessage(sendingMessage);

        using SmtpClient smtpClient = new();
        smtpClient.Connect(options.Value.Host, options.Value.Port, options.Value.UseSsl);
        smtpClient.Authenticate(options.Value.UserName, options.Value.Password);
        smtpClient.Send(mailMessage);
        smtpClient.Disconnect(true);
    }

    private static MimeMessage CreateMailMessage(SendingMessage sendingMessage)
    {
        MimeMessage message = new();

        message.From.Add(new MailboxAddress(
            sendingMessage.FromAddress.MailAddress,
            sendingMessage.FromAddress.DisplayName));

        BodyBuilder builder = new();

        if (sendingMessage.IsHtml)
        {
            builder.HtmlBody = sendingMessage.MailBody;
        }
        else
        {
            builder.TextBody = sendingMessage.MailBody;
        }

        message.Body = builder.ToMessageBody();

        foreach (var (address, displayName) in sendingMessage.ToAddresses)
        {
            message.To.Add(new MailboxAddress(address, displayName));
        }

        foreach (var (address, displayName) in sendingMessage.CcAddresses)
        {
            message.Cc.Add(new MailboxAddress(address, displayName));
        }

        foreach (var (address, displayName) in sendingMessage.BccAddresses)
        {
            message.Bcc.Add(new MailboxAddress(address, displayName));
        }

        foreach (var (address, displayName) in sendingMessage.ReplyToAddresses)
        {
            message.ReplyTo.Add(new MailboxAddress(address, displayName));
        }

        return message;
    }
}
