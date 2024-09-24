namespace KISS.FluentEmail.Senders.ElasticEmail;

/// <summary>
///     Initializes a new instance of the ElasticEmailOptions by using configuration file settings.
/// </summary>
/// <param name="Host">The name or IP address of the host used for SMTP transactions.</param>
/// <param name="Port">The port used for SMTP transactions.</param>
/// <param name="UserName">The username associated with the credentials.</param>
/// <param name="Password">The password for the username associated with the credentials.</param>
public record ElasticEmailOptions(
    string Host,
    int Port,
    string UserName,
    string Password)
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ElasticEmailOptions" /> class.
    /// </summary>
    public ElasticEmailOptions()
        : this(string.Empty, default, string.Empty, string.Empty)
    {
    }
}
