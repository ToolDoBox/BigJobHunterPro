using Application.Interfaces.Data;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for TimelineEvent entity.
/// </summary>
public class TimelineEventRepository : Repository<TimelineEvent>, ITimelineEventRepository
{
    public TimelineEventRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TimelineEvent>> GetByApplicationIdAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.ApplicationId == applicationId)
            .OrderByDescending(te => te.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimelineEvent>> GetByEventTypeAsync(
        Guid applicationId,
        EventType eventType,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.ApplicationId == applicationId && te.EventType == eventType)
            .OrderByDescending(te => te.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<TimelineEvent?> GetMostRecentAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(te => te.ApplicationId == applicationId)
            .OrderByDescending(te => te.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountByApplicationIdAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(te => te.ApplicationId == applicationId, cancellationToken);
    }

    public async Task<int> SumPointsByUserIdInRangeAsync(
        string userId,
        DateTime startInclusive,
        DateTime endExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.Application.UserId == userId
                && e.Timestamp >= startInclusive
                && e.Timestamp < endExclusive)
            .SumAsync(e => e.Points, cancellationToken);
    }
}
