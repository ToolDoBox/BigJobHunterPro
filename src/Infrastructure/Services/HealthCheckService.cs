using System.Diagnostics;
using Application.DTOs.Health;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class HealthCheckService : IHealthCheckService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HealthCheckService> _logger;

    public HealthCheckService(
        ApplicationDbContext dbContext,
        IConfiguration configuration,
        ILogger<HealthCheckService> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var overallStopwatch = Stopwatch.StartNew();
        var result = new HealthCheckResult
        {
            TimestampUtc = DateTime.UtcNow,
            Components = new Dictionary<string, ComponentHealth>()
        };

        // Check database connectivity
        await CheckDatabaseAsync(result, cancellationToken);

        // Check JWT configuration
        CheckJwtConfiguration(result);

        // Check database migrations
        await CheckDatabaseMigrationsAsync(result, cancellationToken);

        // Check critical configuration
        CheckCriticalConfiguration(result);

        // Determine overall status
        result.Status = DetermineOverallStatus(result.Components);

        overallStopwatch.Stop();
        result.Duration = overallStopwatch.Elapsed;

        // Log health check result
        if (result.Status == HealthStatus.Unhealthy)
        {
            _logger.LogWarning(
                "Health check failed: {Status}. Unhealthy components: {Components}",
                result.Status,
                string.Join(", ", result.Components
                    .Where(c => c.Value.Status == HealthStatus.Unhealthy)
                    .Select(c => c.Key)));
        }

        return result;
    }

    private async Task CheckDatabaseAsync(HealthCheckResult result, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var component = new ComponentHealth();

        try
        {
            // Try to connect to the database with a timeout
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            var canConnect = await _dbContext.Database.CanConnectAsync(linkedCts.Token);

            if (canConnect)
            {
                // Try a simple query to verify database is responsive
                var userCount = await _dbContext.Users.CountAsync(linkedCts.Token);

                component.Status = HealthStatus.Healthy;
                component.Description = "Database is connected and responsive";
                component.Data = new Dictionary<string, object>
                {
                    ["provider"] = _dbContext.Database.ProviderName ?? "unknown",
                    ["userCount"] = userCount
                };
            }
            else
            {
                component.Status = HealthStatus.Unhealthy;
                component.Description = "Cannot connect to database";
                component.Error = "Database connection failed";
            }
        }
        catch (OperationCanceledException)
        {
            component.Status = HealthStatus.Unhealthy;
            component.Description = "Database health check timed out";
            component.Error = "Database did not respond within 5 seconds";
        }
        catch (Exception ex)
        {
            component.Status = HealthStatus.Unhealthy;
            component.Description = "Database error";
            component.Error = ex.Message;
            _logger.LogError(ex, "Database health check failed");
        }

        stopwatch.Stop();
        component.Duration = stopwatch.Elapsed;
        result.Components["Database"] = component;
    }

    private void CheckJwtConfiguration(HealthCheckResult result)
    {
        var stopwatch = Stopwatch.StartNew();
        var component = new ComponentHealth();

        try
        {
            var jwtSecret = _configuration["JwtSettings:Secret"];
            var jwtIssuer = _configuration["JwtSettings:Issuer"];
            var jwtAudience = _configuration["JwtSettings:Audience"];

            var issues = new List<string>();

            if (string.IsNullOrWhiteSpace(jwtSecret) ||
                jwtSecret.Contains("PLACEHOLDER", StringComparison.OrdinalIgnoreCase) ||
                jwtSecret.Contains("placeholder", StringComparison.OrdinalIgnoreCase))
            {
                issues.Add("JWT secret is not configured or is a placeholder");
            }
            else if (jwtSecret.Length < 32)
            {
                issues.Add("JWT secret is too short (should be at least 32 characters)");
            }

            if (string.IsNullOrWhiteSpace(jwtIssuer))
            {
                issues.Add("JWT issuer is not configured");
            }

            if (string.IsNullOrWhiteSpace(jwtAudience))
            {
                issues.Add("JWT audience is not configured");
            }

            if (issues.Any())
            {
                component.Status = HealthStatus.Unhealthy;
                component.Description = "JWT configuration is incomplete";
                component.Error = string.Join("; ", issues);
            }
            else
            {
                component.Status = HealthStatus.Healthy;
                component.Description = "JWT authentication is properly configured";
                component.Data = new Dictionary<string, object>
                {
                    ["issuer"] = jwtIssuer!,
                    ["audience"] = jwtAudience!
                };
            }
        }
        catch (Exception ex)
        {
            component.Status = HealthStatus.Unhealthy;
            component.Description = "Error checking JWT configuration";
            component.Error = ex.Message;
            _logger.LogError(ex, "JWT configuration health check failed");
        }

        stopwatch.Stop();
        component.Duration = stopwatch.Elapsed;
        result.Components["JWT"] = component;
    }

    private async Task CheckDatabaseMigrationsAsync(HealthCheckResult result, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var component = new ComponentHealth();

        try
        {
            // Only check migrations if database is accessible
            if (result.Components.TryGetValue("Database", out var dbComponent) &&
                dbComponent.Status == HealthStatus.Healthy)
            {
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync(linkedCts.Token);
                var appliedMigrations = await _dbContext.Database.GetAppliedMigrationsAsync(linkedCts.Token);

                if (pendingMigrations.Any())
                {
                    component.Status = HealthStatus.Degraded;
                    component.Description = "Database has pending migrations";
                    component.Data = new Dictionary<string, object>
                    {
                        ["pendingCount"] = pendingMigrations.Count(),
                        ["pendingMigrations"] = pendingMigrations.ToList()
                    };
                    result.Messages.Add($"Warning: {pendingMigrations.Count()} pending database migration(s)");
                }
                else
                {
                    component.Status = HealthStatus.Healthy;
                    component.Description = "Database schema is up to date";
                    component.Data = new Dictionary<string, object>
                    {
                        ["appliedCount"] = appliedMigrations.Count()
                    };
                }
            }
            else
            {
                component.Status = HealthStatus.Unhealthy;
                component.Description = "Cannot check migrations - database unavailable";
            }
        }
        catch (OperationCanceledException)
        {
            component.Status = HealthStatus.Degraded;
            component.Description = "Migration check timed out";
            component.Error = "Could not verify migrations within 5 seconds";
        }
        catch (Exception ex)
        {
            component.Status = HealthStatus.Degraded;
            component.Description = "Error checking database migrations";
            component.Error = ex.Message;
            _logger.LogWarning(ex, "Database migration health check failed");
        }

        stopwatch.Stop();
        component.Duration = stopwatch.Elapsed;
        result.Components["Migrations"] = component;
    }

    private void CheckCriticalConfiguration(HealthCheckResult result)
    {
        var stopwatch = Stopwatch.StartNew();
        var component = new ComponentHealth();

        try
        {
            var issues = new List<string>();
            var warnings = new List<string>();

            // Check connection string
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString) ||
                connectionString.Contains("PLACEHOLDER", StringComparison.OrdinalIgnoreCase))
            {
                issues.Add("Database connection string is not configured");
            }

            // Check environment
            var environment = _configuration["ASPNETCORE_ENVIRONMENT"];
            if (string.IsNullOrWhiteSpace(environment))
            {
                warnings.Add("ASPNETCORE_ENVIRONMENT is not set");
            }

            // Check allowed origins for CORS
            var allowedOrigins = _configuration.GetSection("AllowedOrigins").Get<string[]>();
            if (allowedOrigins == null || allowedOrigins.Length == 0)
            {
                warnings.Add("No allowed origins configured for CORS");
            }

            if (issues.Any())
            {
                component.Status = HealthStatus.Unhealthy;
                component.Description = "Critical configuration is missing";
                component.Error = string.Join("; ", issues);
            }
            else if (warnings.Any())
            {
                component.Status = HealthStatus.Degraded;
                component.Description = "Some configuration warnings exist";
                component.Data = new Dictionary<string, object>
                {
                    ["warnings"] = warnings
                };
                result.Messages.AddRange(warnings);
            }
            else
            {
                component.Status = HealthStatus.Healthy;
                component.Description = "All critical configuration is present";
                component.Data = new Dictionary<string, object>
                {
                    ["environment"] = environment ?? "not set"
                };
            }
        }
        catch (Exception ex)
        {
            component.Status = HealthStatus.Degraded;
            component.Description = "Error checking configuration";
            component.Error = ex.Message;
            _logger.LogWarning(ex, "Configuration health check failed");
        }

        stopwatch.Stop();
        component.Duration = stopwatch.Elapsed;
        result.Components["Configuration"] = component;
    }

    private static HealthStatus DetermineOverallStatus(Dictionary<string, ComponentHealth> components)
    {
        if (!components.Any())
        {
            return HealthStatus.Unhealthy;
        }

        // If any component is unhealthy, the whole system is unhealthy
        if (components.Values.Any(c => c.Status == HealthStatus.Unhealthy))
        {
            return HealthStatus.Unhealthy;
        }

        // If any component is degraded, the system is degraded
        if (components.Values.Any(c => c.Status == HealthStatus.Degraded))
        {
            return HealthStatus.Degraded;
        }

        return HealthStatus.Healthy;
    }
}
