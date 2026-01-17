using Application.Interfaces.Data;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for ActivityEvent entity.
/// </summary>
public class ActivityEventRepository : Repository<ActivityEvent>, IActivityEventRepository
{
    public ActivityEventRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ActivityEvent>> GetByPartyIdAsync(
        Guid partyId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ae => ae.User)
            .Where(ae => ae.PartyId == partyId)
            .OrderByDescending(ae => ae.CreatedDate)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ActivityEvent>> GetByUserIdAsync(
        string userId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ae => ae.User)
            .Include(ae => ae.Party)
            .Where(ae => ae.UserId == userId)
            .OrderByDescending(ae => ae.CreatedDate)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ActivityEvent>> GetRecentForUserPartiesAsync(
        string userId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        // Get all party IDs the user belongs to
        var userPartyIds = await _context.HuntingPartyMemberships
            .Where(m => m.UserId == userId && m.IsActive)
            .Select(m => m.HuntingPartyId)
            .ToListAsync(cancellationToken);

        // Get recent activity events from those parties
        return await _dbSet
            .Include(ae => ae.User)
            .Include(ae => ae.Party)
            .Where(ae => userPartyIds.Contains(ae.PartyId))
            .OrderByDescending(ae => ae.CreatedDate)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> MilestoneExistsAsync(
        Guid partyId,
        string userId,
        string milestoneLabel,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(
                e => e.PartyId == partyId
                    && e.UserId == userId
                    && e.EventType == ActivityEventType.MilestoneHit
                    && e.RoleTitle == milestoneLabel,
                cancellationToken);
    }
}
