namespace KISS.Job.MassTransit.RabbitMQ;

/// <summary>
/// Represents configuration options for connecting to a RabbitMQ server.
/// </summary>
public record RabbitMqConfigurationOption
{
    /// <summary>
    /// Gets the RabbitMQ host name or IP address.
    /// </summary>
    [Required(ErrorMessage = "Host is required.")]
    public string Host { get; init; } = "localhost";

    /// <summary>
    /// Gets the port number for the RabbitMQ server.
    /// </summary>
    [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535.")]
    public ushort Port { get; init; } = 5672;

    /// <summary>
    /// Gets the virtual host to use when connecting to RabbitMQ.
    /// </summary>
    public string VirtualHost { get; init; } = "/";

    /// <summary>
    /// Gets the username for authenticating with RabbitMQ.
    /// </summary>
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; init; } = "guest";

    /// <summary>
    /// Gets the password for authenticating with RabbitMQ.
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; init; } = "guest";

    /// <summary>
    /// Gets a value indicating whether to use SSL for the RabbitMQ connection.
    /// </summary>
    public bool UseSsl { get; init; }

    /// <summary>
    /// Gets the heartbeat interval (in seconds) for the RabbitMQ connection.
    /// </summary>
    [Range(0, ushort.MaxValue, ErrorMessage = "HeartbeatSeconds must be non-negative.")]
    public ushort HeartbeatSeconds { get; init; } = 60;
}
