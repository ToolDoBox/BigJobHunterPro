using Application.DTOs.Auth;
using Application.Interfaces;
using BigJobHunterPro.Application.DTOs.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IAnalyticsService analyticsService,
        ICurrentUserService currentUserService,
        ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Get top keywords from successful applications (those that reached interview stage or beyond)
    /// </summary>
    /// <remarks>
    /// Analyzes role titles, job descriptions, and skills from applications that led to interviews or offers.
    /// Results are cached for 10 minutes to reduce computational load.
    /// </remarks>
    /// <param name="topCount">Number of top keywords to return (default: 20, max: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of keywords with their frequencies and percentages</returns>
    [HttpGet("keywords")]
    [ProducesResponseType(typeof(List<KeywordFrequency>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<KeywordFrequency>>> GetTopKeywords(
        [FromQuery] int topCount = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt to keywords analytics");
            return Unauthorized(new ErrorResponse("User not authenticated"));
        }

        // Limit topCount to reasonable range
        topCount = Math.Min(Math.Max(topCount, 1), 50);

        try
        {
            var keywords = await _analyticsService.GetTopKeywordsAsync(userId, topCount, cancellationToken);
            return Ok(keywords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving keyword analytics for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse("An error occurred while retrieving keyword analytics"));
        }
    }

    /// <summary>
    /// Get conversion rates by application source/platform
    /// </summary>
    /// <remarks>
    /// Shows which job boards or sources have the highest success rates (leading to interviews/offers).
    /// Results are cached for 10 minutes.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of sources with their conversion rates</returns>
    [HttpGet("conversion-by-source")]
    [ProducesResponseType(typeof(List<ConversionBySource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ConversionBySource>>> GetConversionBySource(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt to conversion analytics");
            return Unauthorized(new ErrorResponse("User not authenticated"));
        }

        try
        {
            var conversions = await _analyticsService.GetConversionBySourceAsync(userId, cancellationToken);
            return Ok(conversions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversion analytics for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse("An error occurred while retrieving conversion analytics"));
        }
    }
}
