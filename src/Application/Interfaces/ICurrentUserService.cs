using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Service for accessing the current authenticated user from HTTP context
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's ID from JWT claims (fast, no database call)
    /// </summary>
    /// <returns>User ID if authenticated, null otherwise</returns>
    string? GetUserId();

    /// <summary>
    /// Gets the full current user object from the database
    /// </summary>
    /// <returns>ApplicationUser if authenticated and found, null otherwise</returns>
    Task<ApplicationUser?> GetCurrentUserAsync();
}
