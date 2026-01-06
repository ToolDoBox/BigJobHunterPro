using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.TimelineEvents;
using Application.DTOs.Auth;
using Application.Interfaces;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/applications/{applicationId:guid}/timeline-events")]
public class TimelineEventsController : ControllerBase
{
    private readonly ITimelineEventService _timelineEventService;

    public TimelineEventsController(ITimelineEventService timelineEventService)
    {
        _timelineEventService = timelineEventService;
    }

    /// <summary>
    /// Creates a new timeline event for an application
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TimelineEventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTimelineEvent(Guid applicationId, [FromBody] CreateTimelineEventRequest request)
    {
        try
        {
            var result = await _timelineEventService.CreateTimelineEventAsync(applicationId, request);

            return CreatedAtAction(
                nameof(GetTimelineEvents),
                new { applicationId },
                result
            );
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Application not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("Validation failed", new List<string> { ex.Message }));
        }
    }

    /// <summary>
    /// Gets all timeline events for an application
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TimelineEventsListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTimelineEvents(Guid applicationId)
    {
        try
        {
            var result = await _timelineEventService.GetTimelineEventsAsync(applicationId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Application not found" });
        }
    }

    /// <summary>
    /// Updates an existing timeline event
    /// </summary>
    [HttpPut("{eventId:guid}")]
    [ProducesResponseType(typeof(TimelineEventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTimelineEvent(Guid applicationId, Guid eventId, [FromBody] UpdateTimelineEventRequest request)
    {
        try
        {
            var result = await _timelineEventService.UpdateTimelineEventAsync(applicationId, eventId, request);

            if (result == null)
            {
                return NotFound(new { error = "Timeline event not found" });
            }

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Application not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("Validation failed", new List<string> { ex.Message }));
        }
    }

    /// <summary>
    /// Deletes a timeline event
    /// </summary>
    [HttpDelete("{eventId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTimelineEvent(Guid applicationId, Guid eventId)
    {
        try
        {
            var deleted = await _timelineEventService.DeleteTimelineEventAsync(applicationId, eventId);

            if (!deleted)
            {
                return NotFound(new { error = "Timeline event not found" });
            }

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Application not found" });
        }
    }
}
