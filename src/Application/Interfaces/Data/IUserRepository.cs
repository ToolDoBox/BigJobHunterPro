using Domain.Entities;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for ApplicationUser entity.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    Task<ApplicationUser?> GetByIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user's display name by their identifier.
    /// </summary>
    Task<string?> GetDisplayNameAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
