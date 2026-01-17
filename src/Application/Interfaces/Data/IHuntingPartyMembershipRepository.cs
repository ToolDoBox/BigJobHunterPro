using Domain.Entities;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for HuntingPartyMembership entity.
/// </summary>
public interface IHuntingPartyMembershipRepository : IRepository<HuntingPartyMembership>
{
    /// <summary>
    /// Gets all memberships for a user.
    /// </summary>
    Task<IEnumerable<HuntingPartyMembership>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active memberships for a user (with party details).
    /// </summary>
    Task<IEnumerable<HuntingPartyMembership>> GetActiveByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all memberships for a party.
    /// </summary>
    Task<IEnumerable<HuntingPartyMembership>> GetByPartyIdAsync(
        Guid partyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific membership by party and user.
    /// </summary>
    Task<HuntingPartyMembership?> GetByPartyAndUserAsync(
        Guid partyId,
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user is already a member of a party.
    /// </summary>
    Task<bool> IsMemberAsync(
        Guid partyId,
        string userId,
        CancellationToken cancellationToken = default);
}
