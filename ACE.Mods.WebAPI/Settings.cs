namespace ACE.Mods.WebAPI;

public class Settings
{
    /// <summary>
    /// Enables output of more verbose logging
    /// </summary>
    public bool EnableLogging { get; set; } = false;

    /// <summary>
    /// Enables output OpenAPI
    /// </summary>
    public bool EnableOpenAPI { get; set; } = false;

    /// <summary>
    /// Enables output SwaggerUI
    /// </summary>
    public bool EnableSwaggerUI { get; set; } = false;

    /// <summary>
    /// Enables output Redoc
    /// </summary>
    public bool EnableRedoc { get; set; } = false;

    /// <summary>
    /// Enables output Scalar
    /// </summary>
    public bool EnableScalar { get; set; } = false;

    /// <summary>
    /// Host address to Bind to WebAPI
    /// </summary>
    public string Host { get; set; } = "127.0.0.1";

    /// <summary>
    /// Port to Bind to WebAPI
    /// </summary>
    public ushort Port { get; set; } = 8080;

    /// <summary>
    /// Base URL to publish with OpenAPI
    /// </summary>
    public string APIBaseUrl { get; set; } = "http://localhost:8080";

    /// <summary>
    /// Enables WebAPI Server raw output to console
    /// </summary>
    public bool OutputToConsole { get; set; } = false;
}
