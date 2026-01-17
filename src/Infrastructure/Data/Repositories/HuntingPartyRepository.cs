using Application.Interfaces.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for HuntingParty entity.
/// </summary>
public class HuntingPartyRepository : Repository<HuntingParty>, IHuntingPartyRepository
{
    public HuntingPartyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<HuntingParty?> GetByInviteCodeAsync(
        string inviteCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.InviteCode == inviteCode, cancellationToken);
    }

    public async Task<IEnumerable<HuntingParty>> GetByCreatorIdAsync(
        string creatorId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.CreatorId == creatorId)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<HuntingParty?> GetWithMembershipsAsync(
        Guid partyId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Memberships)
                .ThenInclude(m => m.User)
            .Include(p => p.Creator)
            .FirstOrDefaultAsync(p => p.Id == partyId, cancellationToken);
    }

    public async Task<bool> InviteCodeExistsAsync(
        string inviteCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(p => p.InviteCode == inviteCode, cancellationToken);
    }
}
