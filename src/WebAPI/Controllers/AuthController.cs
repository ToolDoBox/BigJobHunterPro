using Application.DTOs.Auth;
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
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        ICurrentUserService currentUserService,
        ApplicationDbContext context,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _currentUserService = currentUserService;
        _context = context;
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

    /// <summary>
    /// Authenticate user and issue JWT token
    /// </summary>
    /// <param name="request">Login credentials (email and password)</param>
    /// <returns>User ID, email, JWT token, and token expiration</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        // 1. Validate model state
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new ErrorResponse("Validation failed", errors));
        }

        // 2. Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
            return Unauthorized(new ErrorResponse(
                "Invalid credentials",
                new List<string> { "Invalid email or password" }
            ));
        }

        // 3. Check password with SignInManager (handles lockout automatically)
        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true
        );

        // 4. Handle lockout scenario
        if (result.IsLockedOut)
        {
            _logger.LogWarning("Login failed: Account locked out for user {UserId}", user.Id);
            return Unauthorized(new ErrorResponse(
                "Account locked",
                new List<string> {
                    "Account is locked due to multiple failed login attempts. Please try again in 5 minutes."
                }
            ));
        }

        // 5. Handle invalid password
        if (!result.Succeeded)
        {
            _logger.LogWarning("Login failed: Invalid password for email {Email}", request.Email);
            return Unauthorized(new ErrorResponse(
                "Invalid credentials",
                new List<string> { "Invalid email or password" }
            ));
        }

        // 6. Generate JWT token and return success response
        var token = _jwtTokenService.GenerateToken(user);
        var expiresAt = _jwtTokenService.GetTokenExpiration();

        _logger.LogInformation("User logged in successfully: {Email}", request.Email);

        var response = new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            Token = token,
            ExpiresAt = expiresAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Get current authenticated user's information
    /// </summary>
    /// <returns>User ID, email, display name, points, and statistics</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(GetMeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetMeResponse>> GetMe()
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ErrorResponse(
                "Unauthorized",
                new List<string> { "Unable to identify authenticated user" }
            ));
        }

        // Use projection to avoid N+1 query and improve performance
        var response = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new GetMeResponse
            {
                UserId = u.Id,
                Email = u.Email ?? string.Empty,
                DisplayName = u.DisplayName,
                Points = u.Points,
                TotalPoints = u.TotalPoints,
                ApplicationCount = u.Applications.Count, // DB-side count
                CurrentStreak = u.CurrentStreak,
                LongestStreak = u.LongestStreak,
                LastActivityDate = u.LastActivityDate
            })
            .FirstOrDefaultAsync();

        // Handle edge case: user deleted after JWT issued
        if (response == null)
        {
            _logger.LogWarning("GetMe failed: User not found for authenticated request");
            return NotFound(new ErrorResponse(
                "User not found",
                new List<string> { "The authenticated user no longer exists in the system" }
            ));
        }

        _logger.LogInformation("User retrieved own profile: {UserId}", userId);

        return Ok(response);
    }

    /// <summary>
    /// Logout current user (client-side token deletion)
    /// </summary>
    /// <returns>Success confirmation</returns>
    /// <remarks>
    /// Since JWTs are stateless, logout is primarily handled client-side by deleting the token.
    /// This endpoint provides a server-side confirmation and can be extended for token blacklisting if needed.
    /// </remarks>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public ActionResult Logout()
    {
        // Get current user ID for logging
        var userId = _currentUserService.GetUserId();

        if (!string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation("User logged out: {UserId}", userId);
        }

        // In a JWT-based auth system, logout is primarily client-side
        // The client deletes the token from localStorage
        // Future enhancement: Implement token blacklisting here if needed

        return Ok(new { success = true });
    }
}
