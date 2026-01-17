using Domain.Entities;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for HuntingParty entity.
/// </summary>
public interface IHuntingPartyRepository : IRepository<HuntingParty>
{
    /// <summary>
    /// Gets a hunting party by its invite code.
    /// </summary>
    Task<HuntingParty?> GetByInviteCodeAsync(
        string inviteCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all hunting parties created by a user.
    /// </summary>
    Task<IEnumerable<HuntingParty>> GetByCreatorIdAsync(
        string creatorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a hunting party with its memberships included.
    /// </summary>
    Task<HuntingParty?> GetWithMembershipsAsync(
        Guid partyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an invite code already exists.
    /// </summary>
    Task<bool> InviteCodeExistsAsync(
        string inviteCode,
        CancellationToken cancellationToken = default);
}
