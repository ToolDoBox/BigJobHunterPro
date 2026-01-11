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
}
