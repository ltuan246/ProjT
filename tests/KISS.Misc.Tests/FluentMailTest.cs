namespace KISS.Misc.Tests;

public class FluentMailTest : IDisposable
{
    private ServiceProvider Services { get; init; }

    public FluentMailTest()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();

        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.ConfigureSmtpClientOptions(configuration);
        serviceCollection.ConfigureMailKitOptions(configuration);
        serviceCollection.ConfigureMailtrapOptions(configuration);
        serviceCollection.ConfigureElasticEmailOptions(configuration);
        Services = serviceCollection.BuildServiceProvider();
    }

    public void Dispose()
    {
        Services.Dispose();
        GC.SuppressFinalize(this);
    }

    private static SendingMessage CreateMailMessage()
    {
        const string sender = "fluentmail@protonmail.com";
        const string displayName = "fluentmail";
        const string to = "fluentmail@protonmail.com";
        const string subject = "KISS";
        const string body = "This is a test email sent using C#.Net";

        SendingMessage mailMessage =
            new(new(sender, displayName), subject, body);
        mailMessage.ToAddresses.Add(new(to));

        return mailMessage;
    }

    [Fact]
    public void SendEmail_SmtpSender_SentSuccessfully()
    {
        var smtpClientOptions = Services.GetService<IOptions<SmtpClientOptions>>();

        Assert.NotNull(smtpClientOptions);

        var mailMessage = CreateMailMessage();
        var sender = new SmtpSender(smtpClientOptions);
        var response = sender.Send(mailMessage);

        Assert.True(response.IsSuccessful);
    }

    [Fact]
    public void SendEmail_MailKitSender_SentSuccessfully()
    {
        var mailKitOptions = Services.GetService<IOptions<MailKitOptions>>();

        Assert.NotNull(mailKitOptions);

        var mailMessage = CreateMailMessage();
        var sender = new MailKitSender(mailKitOptions);
        var response = sender.Send(mailMessage);

        Assert.True(response.IsSuccessful);
    }

    [Fact]
    public void SendEmail_MailtrapSender_SentSuccessfully()
    {
        var mailtrapOptions = Services.GetService<IOptions<MailtrapOptions>>();

        Assert.NotNull(mailtrapOptions);

        var mailMessage = CreateMailMessage();
        var sender = new MailtrapSender(mailtrapOptions);
        var response = sender.Send(mailMessage);

        Assert.True(response.IsSuccessful);
    }

    [Fact]
    public void SendEmail_ElasticEmailSender_SentSuccessfully()
    {
        var elasticEmailOptions = Services.GetService<IOptions<ElasticEmailOptions>>();

        Assert.NotNull(elasticEmailOptions);

        var mailMessage = CreateMailMessage();
        var sender = new ElasticEmailSender(elasticEmailOptions);
        var response = sender.Send(mailMessage);

        Assert.True(response.IsSuccessful);
    }
}
