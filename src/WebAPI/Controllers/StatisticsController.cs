using Application.DTOs.Auth;
using Application.Interfaces;
using BigJobHunterPro.Application.DTOs.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<StatisticsController> _logger;

    public StatisticsController(
        IStatisticsService statisticsService,
        ICurrentUserService currentUserService,
        ILogger<StatisticsController> logger)
    {
        _statisticsService = statisticsService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Get weekly statistics comparing this week vs last week
    /// </summary>
    /// <remarks>
    /// Returns a rolling 7-day comparison of applications and points.
    /// Results are cached for 5 minutes to reduce database load.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Weekly statistics with percentage change</returns>
    [HttpGet("weekly")]
    [ProducesResponseType(typeof(WeeklyStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WeeklyStatsResponse>> GetWeeklyStats(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt to weekly statistics");
            return Unauthorized(new ErrorResponse("User not authenticated"));
        }

        try
        {
            var stats = await _statisticsService.GetWeeklyStatsAsync(userId, cancellationToken);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weekly statistics for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse("An error occurred while retrieving statistics"));
        }
    }

    /// <summary>
    /// Get distribution of applications across different statuses
    /// </summary>
    /// <remarks>
    /// Shows how many applications are at each stage (Applied, Screening, Interview, Rejected, Offer).
    /// Results are cached for 5 minutes to reduce database load.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Status distribution data</returns>
    [HttpGet("status-distribution")]
    [ProducesResponseType(typeof(StatusDistributionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StatusDistributionResponse>> GetStatusDistribution(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt to status distribution");
            return Unauthorized(new ErrorResponse("User not authenticated"));
        }

        try
        {
            var distribution = await _statisticsService.GetStatusDistributionAsync(userId, cancellationToken);
            return Ok(distribution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving status distribution for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse("An error occurred while retrieving status distribution"));
        }
    }

    /// <summary>
    /// Get distribution of applications across different sources
    /// </summary>
    /// <remarks>
    /// Shows how many applications came from each source (LinkedIn, Indeed, etc.).
    /// Results are cached for 5 minutes to reduce database load.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Source distribution data</returns>
    [HttpGet("source-distribution")]
    [ProducesResponseType(typeof(SourceDistributionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SourceDistributionResponse>> GetSourceDistribution(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt to source distribution");
            return Unauthorized(new ErrorResponse("User not authenticated"));
        }

        try
        {
            var distribution = await _statisticsService.GetSourceDistributionAsync(userId, cancellationToken);
            return Ok(distribution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving source distribution for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse("An error occurred while retrieving source distribution"));
        }
    }

    /// <summary>
    /// Get average time from application submission to various milestones
    /// </summary>
    /// <remarks>
    /// Shows average days to reach Screening, Interview, Rejection, and Offer stages.
    /// Results are cached for 5 minutes to reduce database load.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Average time to milestone data</returns>
    [HttpGet("average-time")]
    [ProducesResponseType(typeof(AverageTimeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AverageTimeResponse>> GetAverageTime(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt to average time statistics");
            return Unauthorized(new ErrorResponse("User not authenticated"));
        }

        try
        {
            var averageTime = await _statisticsService.GetAverageTimeToMilestonesAsync(userId, cancellationToken);
            return Ok(averageTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving average time statistics for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse("An error occurred while retrieving average time statistics"));
        }
    }
}
