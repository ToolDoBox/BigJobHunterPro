using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="request">Registration details including email, password, and display name</param>
    /// <returns>User ID, email, and JWT token</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        // Validate model state
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new ErrorResponse("Validation failed", errors));
        }

        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest(new ErrorResponse(
                "Email already registered",
                new List<string> { "A user with this email address already exists. Please try logging in or use a different email." }
            ));
        }

        // Create new user
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email, // Using email as username
            DisplayName = request.DisplayName,
            Points = 0,
            CreatedDate = DateTime.UtcNow
        };

        // Create user with password
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("User registration failed for {Email}: {Errors}",
                request.Email, string.Join(", ", errors));

            return BadRequest(new ErrorResponse("Registration failed", errors));
        }

        _logger.LogInformation("User registered successfully: {Email}", request.Email);

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(user);

        var response = new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            Token = token
        };

        return CreatedAtAction(nameof(Register), new { id = user.Id }, response);
    }
}
