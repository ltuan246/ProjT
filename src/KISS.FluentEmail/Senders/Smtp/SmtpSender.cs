using System.Net;
using System.Net.Mail;

namespace KISS.FluentEmail.Senders.Smtp;

/// <summary>
///     Send email via SMTP server.
/// </summary>
public class SmtpSender
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SmtpSender" /> class.
    /// </summary>
    /// <param name="options">An SMTP client options that can be used to send email messages.</param>
    public SmtpSender(IOptions<SmtpClientOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        Sender = new SmtpClient
        {
            Host = options.Value.Host,
            Port = options.Value.Port,
            UseDefaultCredentials = options.Value.UseDefaultCredentials,
            EnableSsl = options.Value.UseSsl
        };

        if (options.Value.UseDefaultCredentials)
        {
            return;
        }

        ArgumentException.ThrowIfNullOrEmpty(options.Value.UserName, nameof(options.Value.UserName));
        ArgumentException.ThrowIfNullOrEmpty(options.Value.Password, nameof(options.Value.Password));

        Sender.Credentials = new NetworkCredential(options.Value.UserName, options.Value.Password);
    }

    private SmtpClient Sender { get; }

    /// <summary>
    ///     Send the specified message.
    /// </summary>
    /// <param name="sendingMessage">Specified message.</param>
    public void Send([NotNull] SendingMessage sendingMessage)
    {
        using var mailMessage = CreateMailMessage(sendingMessage);
        Sender.Send(mailMessage);
    }

    private static MailMessage CreateMailMessage(SendingMessage sendingMessage)
    {
        MailMessage message = new()
        {
            From = new MailAddress(sendingMessage.FromAddress.MailAddress, sendingMessage.FromAddress.DisplayName),
            Subject = sendingMessage.MailSubject,
            Body = sendingMessage.MailBody
        };

        foreach (var (address, displayName) in sendingMessage.ToAddresses)
        {
            message.To.Add(new MailAddress(address, displayName));
        }

        foreach (var (address, displayName) in sendingMessage.CcAddresses)
        {
            message.CC.Add(new MailAddress(address, displayName));
        }

        foreach (var (address, displayName) in sendingMessage.BccAddresses)
        {
            message.Bcc.Add(new MailAddress(address, displayName));
        }

        foreach (var (address, displayName) in sendingMessage.ReplyToAddresses)
        {
            message.ReplyToList.Add(new MailAddress(address, displayName));
        }

        foreach ((var filename, var data, var contentType) in sendingMessage.Attachments)
        {
            message.Attachments.Add(new Attachment(data, filename, contentType));
        }

        return message;
    }
}
