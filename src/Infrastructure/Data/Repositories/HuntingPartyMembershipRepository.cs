using Application.Interfaces.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for HuntingPartyMembership entity.
/// </summary>
public class HuntingPartyMembershipRepository : Repository<HuntingPartyMembership>, IHuntingPartyMembershipRepository
{
    public HuntingPartyMembershipRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<HuntingPartyMembership>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.HuntingParty)
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.JoinedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<HuntingPartyMembership>> GetActiveByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.HuntingParty)
            .Where(m => m.UserId == userId && m.IsActive)
            .OrderByDescending(m => m.JoinedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<HuntingPartyMembership>> GetByPartyIdAsync(
        Guid partyId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.User)
            .Where(m => m.HuntingPartyId == partyId)
            .OrderBy(m => m.JoinedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<HuntingPartyMembership?> GetByPartyAndUserAsync(
        Guid partyId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.HuntingParty)
            .Include(m => m.User)
            .FirstOrDefaultAsync(
                m => m.HuntingPartyId == partyId && m.UserId == userId,
                cancellationToken);
    }

    public async Task<bool> IsMemberAsync(
        Guid partyId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(
                m => m.HuntingPartyId == partyId && m.UserId == userId && m.IsActive,
                cancellationToken);
    }
}
