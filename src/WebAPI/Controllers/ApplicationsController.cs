using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Application.DTOs.Applications;
using Application.DTOs.Auth;
using Application.Exceptions;
using Application.Interfaces;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    /// <summary>
    /// Creates a new job application (Quick Capture)
    /// </summary>
    [HttpPost]
    [EnableRateLimiting("QuickCapture")]
    [RequestSizeLimit(100_000)]
    [ProducesResponseType(typeof(CreateApplicationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest request)
    {
        try
        {
            var result = await _applicationService.CreateApplicationAsync(request);

            return CreatedAtAction(
                nameof(GetApplication),
                new { id = result.Id },
                result
            );
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
        catch (RateLimitExceededException ex)
        {
            return StatusCode(StatusCodes.Status429TooManyRequests, new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("Validation failed", new List<string> { ex.Message }));
        }
    }

    /// <summary>
    /// Gets applications for the current user (paged, newest first)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApplicationsListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetApplications([FromQuery] int page = 1, [FromQuery] int pageSize = 25)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new { error = "Page must be >= 1 and pageSize must be between 1 and 100." });
        }

        try
        {
            var result = await _applicationService.GetApplicationsAsync(page, pageSize);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
    }

    /// <summary>
    /// Gets a single application by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetApplication(Guid id)
    {
        try
        {
            var result = await _applicationService.GetApplicationAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
    }

    /// <summary>
    /// Updates a single application by ID
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateApplication(Guid id, [FromBody] UpdateApplicationRequest request)
    {
        try
        {
            var result = await _applicationService.UpdateApplicationAsync(id, request);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("Validation failed", new List<string> { ex.Message }));
        }
    }

    /// <summary>
    /// Updates application status by ID
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateApplicationStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var result = await _applicationService.UpdateApplicationStatusAsync(id, request);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("Validation failed", new List<string> { ex.Message }));
        }
    }

    /// <summary>
    /// Deletes a single application by ID
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteApplication(Guid id)
    {
        try
        {
            var deleted = await _applicationService.DeleteApplicationAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
    }
}
