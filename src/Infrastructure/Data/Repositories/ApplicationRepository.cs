using Application.Interfaces.Data;
using ApplicationEntity = Domain.Entities.Application;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Application entity.
/// </summary>
public class ApplicationRepository : Repository<ApplicationEntity>, IApplicationRepository
{
    public ApplicationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ApplicationEntity>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApplicationEntity>> GetWithTimelineEventsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.TimelineEvents.OrderByDescending(te => te.Timestamp))
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<ApplicationEntity?> GetByIdWithTimelineAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.TimelineEvents.OrderByDescending(te => te.Timestamp))
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(a => a.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<ApplicationEntity>> GetByStatusAsync(
        string userId,
        ApplicationStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId && a.Status == status)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApplicationEntity>> GetRecentAsync(
        string userId,
        int count,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedDate)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ApplicationEntity>> GetPageByUserIdAsync(
        string userId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ApplicationEntity>> GetPageByUserIdWithFiltersAsync(
        string userId,
        int skip,
        int take,
        string? search = null,
        ApplicationStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(a => a.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(a =>
                a.CompanyName.ToLower().Contains(searchLower) ||
                a.RoleTitle.ToLower().Contains(searchLower));
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        return await query
            .OrderByDescending(a => a.CreatedDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByUserIdWithFiltersAsync(
        string userId,
        string? search = null,
        ApplicationStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(a => a.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(a =>
                a.CompanyName.ToLower().Contains(searchLower) ||
                a.RoleTitle.ToLower().Contains(searchLower));
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<int> CountByUserIdInRangeAsync(
        string userId,
        DateTime startInclusive,
        DateTime endExclusive,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(
                a => a.UserId == userId
                    && a.CreatedDate >= startInclusive
                    && a.CreatedDate < endExclusive,
                cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetCountsByUserIdsAsync(
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default)
    {
        var idList = userIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (idList.Count == 0)
        {
            return new Dictionary<string, int>(StringComparer.Ordinal);
        }

        var counts = await _dbSet
            .AsNoTracking()
            .Where(a => idList.Contains(a.UserId))
            .GroupBy(a => a.UserId)
            .Select(group => new { group.Key, Count = group.Count() })
            .ToListAsync(cancellationToken);

        return counts.ToDictionary(item => item.Key, item => item.Count, StringComparer.Ordinal);
    }
}
