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
        // mailtrap
        // "Username": "hemax57423@marchub.com",
        // "Password": "hemax57423",
        var smtpClientOptions = ServiceProvider.GetService<IOptions<SmtpClientOptions>>();
        Assert.NotNull(smtpClientOptions);

        var fm = new SmtpSender(smtpClientOptions);
        fm.Send();
    }
}
