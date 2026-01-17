using Application.DTOs.Health;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHealthCheckService _healthCheckService;

    public HealthController(
        ApplicationDbContext dbContext,
        IHealthCheckService healthCheckService)
    {
        _dbContext = dbContext;
        _healthCheckService = healthCheckService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            timestampUtc = DateTime.UtcNow
        });
    }

    [HttpGet("db")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDatabaseHealth(CancellationToken cancellationToken)
    {
        var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

        var response = new
        {
            status = canConnect ? "ok" : "unhealthy",
            database = canConnect ? "connected" : "unreachable",
            timestampUtc = DateTime.UtcNow
        };

        if (!canConnect)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Comprehensive readiness check that validates all critical system components.
    /// Returns 200 OK only when the system is fully ready to handle requests.
    /// </summary>
    /// <remarks>
    /// This endpoint should be used by:
    /// - Azure App Service health check configuration
    /// - Frontend status indicators
    /// - Load balancers and monitoring systems
    ///
    /// Status codes:
    /// - 200 OK: System is healthy or degraded (serving traffic with warnings)
    /// - 503 Service Unavailable: System is unhealthy
    /// </remarks>
    [HttpGet("ready")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(HealthCheckResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthCheckResult), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetReadiness(CancellationToken cancellationToken)
    {
        var healthResult = await _healthCheckService.CheckHealthAsync(cancellationToken);

        // Return 503 only when the system is unhealthy
        if (healthResult.Status == HealthStatus.Unhealthy)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, healthResult);
        }

        return Ok(healthResult);
    }
}
