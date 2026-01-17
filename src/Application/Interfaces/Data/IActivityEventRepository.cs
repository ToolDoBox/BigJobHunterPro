using Domain.Entities;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for ActivityEvent entity.
/// </summary>
public interface IActivityEventRepository : IRepository<ActivityEvent>
{
    /// <summary>
    /// Gets recent activity events for a hunting party.
    /// </summary>
    Task<IEnumerable<ActivityEvent>> GetByPartyIdAsync(
        Guid partyId,
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activity events for a specific user.
    /// </summary>
    Task<IEnumerable<ActivityEvent>> GetByUserIdAsync(
        string userId,
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent activity events across all parties a user belongs to.
    /// </summary>
    Task<IEnumerable<ActivityEvent>> GetRecentForUserPartiesAsync(
        string userId,
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a milestone event already exists for the given party and user.
    /// </summary>
    Task<bool> MilestoneExistsAsync(
        Guid partyId,
        string userId,
        string milestoneLabel,
        CancellationToken cancellationToken = default);
}
