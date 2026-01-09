namespace Application.DTOs.Health;

public class HealthCheckResult
{
    /// <summary>
    /// Overall health status of the system
    /// </summary>
    public HealthStatus Status { get; set; }

    /// <summary>
    /// Timestamp when the health check was performed
    /// </summary>
    public DateTime TimestampUtc { get; set; }

    /// <summary>
    /// Individual component health statuses
    /// </summary>
    public Dictionary<string, ComponentHealth> Components { get; set; } = new();

    /// <summary>
    /// Additional messages or warnings
    /// </summary>
    public List<string> Messages { get; set; } = new();

    /// <summary>
    /// Total time taken to perform the health check
    /// </summary>
    public TimeSpan Duration { get; set; }
}

public class ComponentHealth
{
    /// <summary>
    /// Health status of this component
    /// </summary>
    public HealthStatus Status { get; set; }

    /// <summary>
    /// Human-readable description of the component status
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Error message if the component is unhealthy
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Time taken to check this component
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Additional component-specific data
    /// </summary>
    public Dictionary<string, object>? Data { get; set; }
}

public enum HealthStatus
{
    /// <summary>
    /// System is healthy and fully operational
    /// </summary>
    Healthy,

    /// <summary>
    /// System is degraded but still functional
    /// </summary>
    Degraded,

    /// <summary>
    /// System is unhealthy and may not function correctly
    /// </summary>
    Unhealthy
}
