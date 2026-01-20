using Application.DTOs.Auth;
using Application.DTOs.Profile;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProfileController> _logger;

    private const int MaxResumeLength = 50000;

    public ProfileController(
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService,
        ApplicationDbContext context,
        ILogger<ProfileController> logger)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's profile including resume
    /// </summary>
    /// <returns>Profile data with display name, email, and resume information</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileResponse>> GetProfile()
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ErrorResponse(
                "Unauthorized",
                new List<string> { "Unable to identify authenticated user" }
            ));
        }

        var response = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new ProfileResponse
            {
                DisplayName = u.DisplayName,
                Email = u.Email ?? string.Empty,
                ResumeText = u.ResumeText,
                ResumeUpdatedAt = u.ResumeUpdatedAt,
                CharacterCount = u.ResumeText != null ? u.ResumeText.Length : 0
            })
            .FirstOrDefaultAsync();

        if (response == null)
        {
            _logger.LogWarning("GetProfile failed: User not found for authenticated request");
            return NotFound(new ErrorResponse(
                "User not found",
                new List<string> { "The authenticated user no longer exists in the system" }
            ));
        }

        return Ok(response);
    }

    /// <summary>
    /// Update user's resume text
    /// </summary>
    /// <param name="request">Resume text to save (max 50,000 characters)</param>
    /// <returns>Success status with updated timestamp and character count</returns>
    [HttpPut("resume")]
    [ProducesResponseType(typeof(UpdateResumeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateResumeResponse>> UpdateResume([FromBody] UpdateResumeRequest request)
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ErrorResponse(
                "Unauthorized",
                new List<string> { "Unable to identify authenticated user" }
            ));
        }

        // Validate length
        if (request.ResumeText != null && request.ResumeText.Length > MaxResumeLength)
        {
            return BadRequest(new ErrorResponse(
                "Resume too long",
                new List<string> { $"Resume text cannot exceed {MaxResumeLength:N0} characters. Current length: {request.ResumeText.Length:N0}" }
            ));
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("UpdateResume failed: User not found for authenticated request");
            return NotFound(new ErrorResponse(
                "User not found",
                new List<string> { "The authenticated user no longer exists in the system" }
            ));
        }

        // Update resume
        user.ResumeText = request.ResumeText;
        user.ResumeUpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogError("UpdateResume failed for user {UserId}: {Errors}", userId, string.Join(", ", errors));
            return BadRequest(new ErrorResponse("Failed to update resume", errors));
        }

        _logger.LogInformation("User {UserId} updated their resume ({CharCount} characters)", userId, request.ResumeText?.Length ?? 0);

        return Ok(new UpdateResumeResponse
        {
            Success = true,
            ResumeUpdatedAt = user.ResumeUpdatedAt,
            CharacterCount = request.ResumeText?.Length ?? 0,
            Message = "Resume saved successfully"
        });
    }

    /// <summary>
    /// Clear user's resume text
    /// </summary>
    /// <returns>Success status confirming resume was cleared</returns>
    [HttpDelete("resume")]
    [ProducesResponseType(typeof(UpdateResumeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateResumeResponse>> ClearResume()
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ErrorResponse(
                "Unauthorized",
                new List<string> { "Unable to identify authenticated user" }
            ));
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("ClearResume failed: User not found for authenticated request");
            return NotFound(new ErrorResponse(
                "User not found",
                new List<string> { "The authenticated user no longer exists in the system" }
            ));
        }

        // Clear resume
        user.ResumeText = null;
        user.ResumeUpdatedAt = null;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogError("ClearResume failed for user {UserId}: {Errors}", userId, string.Join(", ", errors));
            return BadRequest(new ErrorResponse("Failed to clear resume", errors));
        }

        _logger.LogInformation("User {UserId} cleared their resume", userId);

        return Ok(new UpdateResumeResponse
        {
            Success = true,
            ResumeUpdatedAt = null,
            CharacterCount = 0,
            Message = "Resume cleared successfully"
        });
    }
}
