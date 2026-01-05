using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Applications;
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
    [ProducesResponseType(typeof(CreateApplicationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest request)
    {
        var result = await _applicationService.CreateApplicationAsync(request);

        return CreatedAtAction(
            nameof(GetApplication),
            new { id = result.Id },
            result
        );
    }

    /// <summary>
    /// Gets a single application by ID (placeholder for Story 3)
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetApplication(Guid id)
    {
        // Placeholder for Story 3
        return NotFound();
    }
}
