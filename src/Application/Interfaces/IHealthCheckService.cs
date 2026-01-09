using Application.DTOs.Health;

namespace Application.Interfaces;

public interface IHealthCheckService
{
    /// <summary>
    /// Performs a comprehensive health check of all critical system components.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health check result with status of all components</returns>
    Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
}
