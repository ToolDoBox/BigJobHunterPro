using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.CoverLetter;
using Application.Interfaces;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/applications/{applicationId:guid}/cover-letter")]
public class CoverLetterController : ControllerBase
{
    private readonly ICoverLetterService _coverLetterService;

    public CoverLetterController(ICoverLetterService coverLetterService)
    {
        _coverLetterService = coverLetterService;
    }

    /// <summary>
    /// Generates a cover letter for the specified application using AI.
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(CoverLetterResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateCoverLetter(
        Guid applicationId,
        [FromBody] GenerateCoverLetterRequest request)
    {
        try
        {
            var result = await _coverLetterService.GenerateCoverLetterAsync(
                applicationId,
                request.ResumeText);

            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
    }

    /// <summary>
    /// Saves or updates the cover letter for the specified application.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(CoverLetterResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SaveCoverLetter(
        Guid applicationId,
        [FromBody] SaveCoverLetterRequest request)
    {
        try
        {
            var result = await _coverLetterService.SaveCoverLetterAsync(
                applicationId,
                request.CoverLetterHtml);

            if (!result.Success)
            {
                if (result.ErrorMessage?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return NotFound(new { error = result.ErrorMessage });
                }
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Your session expired. Please log in again." });
        }
    }
}
