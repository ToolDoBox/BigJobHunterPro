using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Questions;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Enums;

namespace WebAPI.Controllers;

/// <summary>
/// Controller for managing interview questions in the Question Range feature.
/// </summary>
[Authorize]
[ApiController]
[Route("api/interview-questions")]
public class InterviewQuestionsController : ControllerBase
{
    private readonly IInterviewQuestionService _questionService;

    public InterviewQuestionsController(IInterviewQuestionService questionService)
    {
        _questionService = questionService;
    }

    /// <summary>
    /// Gets all interview questions for the current user with optional filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(InterviewQuestionsListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetQuestions(
        [FromQuery] QuestionCategory? category = null,
        [FromQuery] string? search = null,
        [FromQuery] Guid? applicationId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "createdDate",
        [FromQuery] bool sortDescending = true)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var result = await _questionService.GetAllAsync(
                category,
                search,
                applicationId,
                page,
                pageSize,
                sortBy,
                sortDescending);

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
    }

    /// <summary>
    /// Gets a specific interview question by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(InterviewQuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQuestion(Guid id)
    {
        try
        {
            var result = await _questionService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { error = "Question not found" });
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
    }

    /// <summary>
    /// Creates a new interview question.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InterviewQuestionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateQuestion([FromBody] CreateInterviewQuestionRequest request)
    {
        try
        {
            var result = await _questionService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetQuestion),
                new { id = result.Id },
                result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("Validation failed", new List<string> { ex.Message }));
        }
    }

    /// <summary>
    /// Updates an existing interview question.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(InterviewQuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] UpdateInterviewQuestionRequest request)
    {
        try
        {
            var result = await _questionService.UpdateAsync(id, request);

            if (result == null)
            {
                return NotFound(new { error = "Question not found" });
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse("Validation failed", new List<string> { ex.Message }));
        }
    }

    /// <summary>
    /// Deletes an interview question.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestion(Guid id)
    {
        try
        {
            var deleted = await _questionService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new { error = "Question not found" });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
    }

    /// <summary>
    /// Increments the TimesAsked counter for a question.
    /// Call this when the user records that a question was asked again.
    /// </summary>
    [HttpPost("{id:guid}/increment")]
    [ProducesResponseType(typeof(InterviewQuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> IncrementTimesAsked(Guid id)
    {
        try
        {
            var result = await _questionService.IncrementTimesAskedAsync(id);

            if (result == null)
            {
                return NotFound(new { error = "Question not found" });
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
    }

    /// <summary>
    /// Gets the most frequently asked questions for the current user.
    /// </summary>
    [HttpGet("frequent")]
    [ProducesResponseType(typeof(IEnumerable<InterviewQuestionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFrequentQuestions([FromQuery] int limit = 10)
    {
        try
        {
            if (limit < 1) limit = 10;
            if (limit > 50) limit = 50;

            var result = await _questionService.GetFrequentAsync(limit);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
    }
}
