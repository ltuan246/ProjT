namespace KISS.FluentEmail.Senders.Smtp;

/// <summary>
///     SmtpSender.
/// </summary>
public class SmtpSender
{
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

    public void Send()
    {
        using var mailMessage = CreateMailMessage();
        Sender.Send(mailMessage);
    }

    private MailMessage CreateMailMessage()
    {
        MailMessage mailMessage = new();
        mailMessage.From = new MailAddress("from@example.com");
        mailMessage.To.Add("yenilay182@nastyx.com");
        mailMessage.Subject = "Hello world";
        mailMessage.Body = "This is a test email sent using C#.Net";
        return mailMessage;
    }
}
