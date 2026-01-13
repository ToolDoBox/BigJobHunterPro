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

    public async Task<StatusDistributionResponse> GetStatusDistributionAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Generate cache key
        var cacheKey = $"status-distribution-{userId}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out StatusDistributionResponse? cachedData) && cachedData != null)
        {
            return cachedData;
        }

        // Get all applications with their current status
        var statusCounts = await _context.Applications
            .Where(a => a.UserId == userId)
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var totalApplications = statusCounts.Sum(s => s.Count);

        var distributions = statusCounts
            .Select(s => new StatusDistribution
            {
                Status = s.Status.ToString(),
                Count = s.Count,
                Percentage = totalApplications > 0 ? Math.Round((decimal)s.Count / totalApplications * 100, 1) : 0
            })
            .OrderByDescending(s => s.Count)
            .ToList();

        var response = new StatusDistributionResponse
        {
            Statuses = distributions,
            TotalApplications = totalApplications
        };

        // Cache for 5 minutes
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };

        _cache.Set(cacheKey, response, cacheOptions);

        return response;
    }

    public async Task<SourceDistributionResponse> GetSourceDistributionAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Generate cache key
        var cacheKey = $"source-distribution-{userId}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out SourceDistributionResponse? cachedData) && cachedData != null)
        {
            return cachedData;
        }

        // Prefer the parsed/entered source name, fall back to URL host when missing
        var sourceEntries = await _context.Applications
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .Select(a => new { a.SourceName, a.SourceUrl })
            .ToListAsync(cancellationToken);

        var sourceCounts = sourceEntries
            .Select(entry => NormalizeSourceName(entry.SourceName, entry.SourceUrl))
            .GroupBy(sourceName => sourceName)
            .Select(group => new { SourceName = group.Key, Count = group.Count() })
            .OrderByDescending(group => group.Count)
            .ToList();

        var totalApplications = sourceCounts.Sum(s => s.Count);

        var distributions = sourceCounts
            .Select(s => new SourceDistribution
            {
                SourceName = s.SourceName,
                Count = s.Count,
                Percentage = totalApplications > 0 ? Math.Round((decimal)s.Count / totalApplications * 100, 1) : 0
            })
            .OrderByDescending(s => s.Count)
            .ToList();

        var response = new SourceDistributionResponse
        {
            Sources = distributions,
            TotalApplications = totalApplications
        };

        // Cache for 5 minutes
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };

        _cache.Set(cacheKey, response, cacheOptions);

        return response;
    }

    public async Task<AverageTimeResponse> GetAverageTimeToMilestonesAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Generate cache key
        var cacheKey = $"average-time-{userId}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out AverageTimeResponse? cachedData) && cachedData != null)
        {
            return cachedData;
        }

        // Get all applications with their timeline events
        var applications = await _context.Applications
            .Where(a => a.UserId == userId)
            .Include(a => a.TimelineEvents)
            .ToListAsync(cancellationToken);

        var milestones = new List<AverageTimeToMilestone>();

        // Calculate average time to each milestone type
        var milestoneTypes = new[]
        {
            Domain.Enums.EventType.Screening,
            Domain.Enums.EventType.Interview,
            Domain.Enums.EventType.Rejected,
            Domain.Enums.EventType.Offer
        };

        foreach (var milestoneType in milestoneTypes)
        {
            var applicationsWithMilestone = applications
                .Select(app => new
                {
                    Application = app,
                    MilestoneEvent = app.TimelineEvents
                        .Where(e => e.EventType == milestoneType)
                        .OrderBy(e => e.Timestamp)
                        .FirstOrDefault()
                })
                .Where(x => x.MilestoneEvent != null)
                .ToList();

            if (applicationsWithMilestone.Any())
            {
                var averageDays = applicationsWithMilestone
                    .Select(x => (x.MilestoneEvent!.Timestamp - x.Application.CreatedDate).TotalDays)
                    .Average();

                milestones.Add(new AverageTimeToMilestone
                {
                    Milestone = milestoneType.ToString(),
                    AverageDays = Math.Round((decimal)averageDays, 1),
                    SampleSize = applicationsWithMilestone.Count
                });
            }
            else
            {
                milestones.Add(new AverageTimeToMilestone
                {
                    Milestone = milestoneType.ToString(),
                    AverageDays = null,
                    SampleSize = 0
                });
            }
        }

        var response = new AverageTimeResponse
        {
            Milestones = milestones
        };

        // Cache for 5 minutes
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };

        _cache.Set(cacheKey, response, cacheOptions);

        return response;
    }

    private static string NormalizeSourceName(string? sourceName, string? sourceUrl)
    {
        if (!string.IsNullOrWhiteSpace(sourceName))
        {
            return sourceName.Trim();
        }

        if (string.IsNullOrWhiteSpace(sourceUrl))
        {
            return "Unknown";
        }

        if (!Uri.TryCreate(sourceUrl, UriKind.Absolute, out var uri))
        {
            return "Unknown";
        }

        var host = uri.Host;
        if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
        {
            host = host[4..];
        }

        return string.IsNullOrWhiteSpace(host) ? "Unknown" : host;
    }
}
