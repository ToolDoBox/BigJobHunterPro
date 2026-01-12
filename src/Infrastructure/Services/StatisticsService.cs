using Application.Interfaces;
using BigJobHunterPro.Application.DTOs.Statistics;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

/// <summary>
/// Service for calculating user statistics and metrics
/// Implements caching to reduce database load
/// </summary>
public class StatisticsService : IStatisticsService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const int CacheExpirationMinutes = 5;

    public StatisticsService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<WeeklyStatsResponse> GetWeeklyStatsAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Generate cache key
        var cacheKey = $"weekly-stats-{userId}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out WeeklyStatsResponse? cachedStats) && cachedStats != null)
        {
            return cachedStats;
        }

        // Calculate statistics
        var now = DateTime.UtcNow;
        var oneWeekAgo = now.AddDays(-7);
        var twoWeeksAgo = now.AddDays(-14);

        // Get applications for this week (last 7 days)
        var applicationsThisWeek = await _context.Applications
            .Where(a => a.UserId == userId && a.CreatedDate >= oneWeekAgo)
            .ToListAsync(cancellationToken);

        // Get applications for last week (7-14 days ago)
        var applicationsLastWeek = await _context.Applications
            .Where(a => a.UserId == userId && a.CreatedDate >= twoWeeksAgo && a.CreatedDate < oneWeekAgo)
            .ToListAsync(cancellationToken);

        // Calculate points for this week (sum all timeline events)
        var pointsThisWeek = await _context.TimelineEvents
            .Where(e => e.Application.UserId == userId && e.Timestamp >= oneWeekAgo)
            .SumAsync(e => e.Points, cancellationToken);

        // Calculate points for last week
        var pointsLastWeek = await _context.TimelineEvents
            .Where(e => e.Application.UserId == userId && e.Timestamp >= twoWeeksAgo && e.Timestamp < oneWeekAgo)
            .SumAsync(e => e.Points, cancellationToken);

        // Calculate percentage change
        var percentageChange = applicationsLastWeek.Count == 0
            ? (applicationsThisWeek.Count > 0 ? 100m : 0m)
            : ((applicationsThisWeek.Count - applicationsLastWeek.Count) / (decimal)applicationsLastWeek.Count * 100);

        var stats = new WeeklyStatsResponse
        {
            ApplicationsThisWeek = applicationsThisWeek.Count,
            ApplicationsLastWeek = applicationsLastWeek.Count,
            PercentageChange = Math.Round(percentageChange, 1),
            PointsThisWeek = pointsThisWeek,
            PointsLastWeek = pointsLastWeek
        };

        // Cache for 5 minutes
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };

        _cache.Set(cacheKey, stats, cacheOptions);

        return stats;
    }
}
