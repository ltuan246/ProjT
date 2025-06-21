namespace KISS.Caching.Stores.Redis;

/// <summary>
///     Represents the configuration settings required to connect and interact with a Redis server.
/// </summary>
public sealed record RedisConfigurationOption
{
    /// <summary>
    ///     Gets or sets the list of Redis server endpoints (host:port).
    /// </summary>
    public required IReadOnlyList<string> EndPoints { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to abort the connection if the initial connect fails.
    /// </summary>
    public bool AbortOnConnectFail { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether admin operations are allowed.
    /// </summary>
    public bool AllowAdmin { get; set; }

    /// <summary>
    ///     Gets or sets the client name to use when connecting to Redis.
    /// </summary>
    public string ClientName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the number of times to retry connecting before giving up.
    /// </summary>
    public int ConnectRetry { get; set; } = 3;

    /// <summary>
    ///     Gets or sets the timeout (in milliseconds) for connect operations.
    /// </summary>
    public int ConnectTimeout { get; set; } = 5000;

    /// <summary>
    ///     Gets or sets the default database index to use when connecting.
    /// </summary>
    public int? DefaultDatabase { get; set; }

    /// <summary>
    ///     Gets or sets the TCP keep-alive time (in seconds).
    /// </summary>
    public int KeepAlive { get; set; } = 60;

    /// <summary>
    ///     Gets or sets the password used to authenticate with the Redis server.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the proxy server address, if applicable.
    /// </summary>
    public string Proxy { get; set; } = "None";

    /// <summary>
    ///     Gets or sets a value indicating whether to resolve DNS endpoints.
    /// </summary>
    public bool ResolveDns { get; set; }

    /// <summary>
    ///     Gets or sets the service name for Redis Sentinel or clustering.
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets a value indicating whether to use SSL for the connection.
    /// </summary>
    public bool Ssl { get; set; }

    /// <summary>
    ///     Gets or sets the expected SSL host name.
    /// </summary>
    public string SslHost { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the timeout (in milliseconds) for synchronous operations.
    /// </summary>
    public int SyncTimeout { get; set; } = 10000;

    /// <summary>
    ///     Gets or sets the value used for tie-breaking in Redis Sentinel scenarios.
    /// </summary>
    public string TieBreaker { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the user name for authentication.
    /// </summary>
    public string User { get; set; } = string.Empty;
}
