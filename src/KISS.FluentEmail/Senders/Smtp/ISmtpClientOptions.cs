namespace KISS.FluentEmail.Senders.Smtp;

/// <summary>
///     An interface that defines the SmtpClientOptions.
/// </summary>
public interface ISmtpClientOptions
{
    /// <summary>
    ///     The name or IP address of the host used for SMTP transactions.
    /// </summary>
    string Host { get; }

    /// <summary>
    ///     The port used for SMTP transactions.
    /// </summary>
    int Port { get; }

    /// <summary>
    ///     A value that controls whether the DefaultCredentials are sent with requests.
    /// </summary>
    bool UseDefaultCredentials { get; }

    /// <summary>
    ///     The username associated with the credentials.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    ///     The password for the username associated with the credentials.
    /// </summary>
    string? Password { get; }

    /// <summary>
    ///     Specify whether the SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection.
    /// </summary>
    bool UseSsl { get; }
}
