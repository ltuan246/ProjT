namespace KISS.FluentEmail.Senders.Smtp;

/// <summary>
///     Initializes a new instance of the SmtpClientOptions by using configuration file settings.
/// </summary>
/// <param name="Host">The name or IP address of the host used for SMTP transactions.</param>
/// <param name="Port">The port used for SMTP transactions.</param>
/// <param name="UseDefaultCredentials">A value that controls whether the DefaultCredentials are sent with requests.</param>
/// <param name="UserName">The username associated with the credentials.</param>
/// <param name="Password">The password for the username associated with the credentials.</param>
/// <param name="UseSsl">Specify whether the SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection.</param>
public record SmtpClientOptions(
    string Host,
    int Port,
    bool UseDefaultCredentials,
    string? UserName,
    string? Password,
    bool UseSsl)
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SmtpClientOptions" /> class.
    /// </summary>
    public SmtpClientOptions()
        : this(string.Empty, default, default, default, default, default)
    {
    }
}
