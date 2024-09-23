namespace KISS.Misc.Tests;

public class FluentEmailTest : IDisposable
{
    private ServiceProvider ServiceProvider { get; init; }

    public FluentEmailTest()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();

        IServiceCollection services = new ServiceCollection();
        services.ConfigureSmtpClientOptions(configuration);
        services.ConfigureMailKitOptions(configuration);
        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        ServiceProvider.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void SendEmail_SmtpSender_CanSendEmail()
    {
        var smtpClientOptions = ServiceProvider.GetService<IOptions<SmtpClientOptions>>();

        Assert.NotNull(smtpClientOptions);

        SendingMessage mailMessage =
            new(new("from@example.com"), "Hello world", "This is a test email sent using C#.Net");
        mailMessage.ToAddresses.Add(new("yenilay182@nastyx.com"));

        var sender = new SmtpSender(smtpClientOptions);
        sender.Send(mailMessage);
    }

    [Fact]
    public void SendEmail_MailKitSender_CanSendEmail()
    {
        var mailKitOptions = ServiceProvider.GetService<IOptions<MailKitOptions>>();

        Assert.NotNull(mailKitOptions);

        SendingMessage mailMessage =
            new(new("from@example.com"), "Hello world", "This is a test email sent using C#.Net");
        mailMessage.ToAddresses.Add(new("yenilay182@nastyx.com"));

        var sender = new MailKitSender(mailKitOptions);
        sender.Send(mailMessage);
    }
}
