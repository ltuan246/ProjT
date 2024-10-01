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
    /// <param name="options">The SmtpClient class is used to send email to an SMTP server for delivery.</param>
    public SmtpSender([NotNull] IOptions<SmtpClientOptions> options) => Options = options.Value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SmtpSender" /> class.
    /// </summary>
    /// <param name="options">The SmtpClient class is used to send email to an SMTP server for delivery.</param>
    public SmtpSender(ISmtpClientOptions options) => Options = options;

    private ISmtpClientOptions Options { get; }

    /// <summary>
    ///     Send the specified message.
    /// </summary>
    /// <param name="sendingMessage">Specified message.</param>
    /// <returns>SendResponse.</returns>
    public SendResponse Send([NotNull] SendingMessage sendingMessage)
    {
        try
        {
            using var mailMessage = CreateMailMessage(sendingMessage);

            using SmtpClient sender = new();
            sender.Host = Options.Host;
            sender.Port = Options.Port;
            sender.UseDefaultCredentials = Options.UseDefaultCredentials;
            sender.Credentials = new NetworkCredential(Options.UserName, Options.Password);
            sender.EnableSsl = Options.UseSsl;
            sender.Send(mailMessage);

            return new SendResponse();
        }
        catch (Exception e)
        {
            SendResponse response = new();
            response.ErrorMessages.Add(e.Message);
            return response;
        }
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

        foreach (var (filename, data, contentType) in sendingMessage.Attachments)
        {
            message.Attachments.Add(new Attachment(data, filename, contentType));
        }

        return message;
    }
}
